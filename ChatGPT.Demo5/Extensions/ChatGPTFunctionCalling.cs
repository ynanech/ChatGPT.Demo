using System.Text.Json;
using OpenAI.ObjectModels.RequestModels;

namespace ChatGPT.Demo5.Extensions
{
    public class ChatGPTFunctionCalling
    {
        private static int _queueNumber = 0;
        private static List<RegistrationInfo> _registrationInfos = new List<RegistrationInfo>();

        //挂号
        public static string Register(string phoneNumber, DateOnly registrationTime)
        {
            if (_registrationInfos.Any(m => m.PhoneNumber == phoneNumber && m.RegistrationTime == registrationTime))
                return JsonSerializer.Serialize(new RegistrationStatus
                {
                    Status = false,
                    Message = "请勿重复挂号",
                });

            var registrationInfo = new RegistrationInfo
            {
                QueueNumber = ++_queueNumber,
                PhoneNumber = phoneNumber,
                RegistrationTime = registrationTime,
                SubmissionTime = DateTime.Now
            };

            _registrationInfos.Add(registrationInfo);
            // 获取给定位置的当前天气信息
            return JsonSerializer.Serialize(new RegistrationStatus
            {
                Status = true,
                Message = "挂号成功",
                RegistrationInfo = registrationInfo
            });
        }

        //查询
        public static string Query(string phoneNumber, DateOnly registrationTime)
        {
            var registrationInfo = _registrationInfos.FirstOrDefault(m => m.PhoneNumber == phoneNumber && m.RegistrationTime == registrationTime);
            var registrationStatus = new RegistrationStatus
            {
                Status = registrationInfo != null,
                Message = registrationInfo != null ? "查询到挂号信息" : "未查询到挂号信息",
                RegistrationInfo = registrationInfo
            };
            return JsonSerializer.Serialize(registrationStatus);
        }


        public static Dictionary<string, Func<string, DateOnly, string>> AvailableFunctions = new()
        {
            { "gpt_register", Register },
            { "gpt_query", Query }
        };

        //JSON Schema
        public static List<FunctionDefinition> Functions = new List<FunctionDefinition>
        {
            new FunctionDefinition
            {
                Name = "gpt_register",
                Description = "通过指定的手机号码和日期进行挂号",
                Parameters = new FunctionParameters
                {
                    Type = "object",
                    Properties = new Dictionary<string, FunctionParameterPropertyValue>
                    {
                        {
                            "phoneNumber",new FunctionParameterPropertyValue
                            {
                                Type = "string",
                                Description = "挂号使用的手机号码"
                            }
                        },
                        {
                            "registrationTime" ,new FunctionParameterPropertyValue
                            {
                                Type = "string",
                                Description = "挂号的日期，比如：明天",
                                Enum = new[] { "今天","明天","后天" }
                            }
                        }
                    },
                    Required = new[] { "phoneNumber", "registrationTime" }
                },
            },
            new FunctionDefinition
            {
                Name = "gpt_query",
                Description = "根据手机号码查询挂号信息",
                Parameters = new FunctionParameters
                {
                    Type = "object",
                    Properties = new Dictionary<string, FunctionParameterPropertyValue>
                    {
                        {
                            "phoneNumber",new FunctionParameterPropertyValue
                            {
                                Type = "string",
                                Description = "要查询的手机号码"
                            }
                        },
                        {
                            "registrationTime" ,new FunctionParameterPropertyValue
                            {
                                Type = "string",
                                Description = "挂号的日期，比如：明天",
                                Enum = new[] { "今天","明天","后天" }
                            }
                        }
                    },
                    Required = new[] { "phoneNumber", "registrationTime" }
                }
            }
        };
    }

    public class RegistrationStatus
    {
        /// <summary>
        /// 挂号状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 状态说明
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 挂号信息
        /// </summary>
        public RegistrationInfo RegistrationInfo { get; set; }
    }

    public class RegistrationInfo
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int QueueNumber { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 挂号时间
        /// </summary>
        public DateOnly RegistrationTime { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        public DateTime SubmissionTime { get; set; }
    }
}
