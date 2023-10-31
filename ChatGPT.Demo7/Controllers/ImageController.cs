using Microsoft.AspNetCore.Mvc;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace ChatGPT.Demo7.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IOpenAIService _openAiService;

        public ImageController(IOpenAIService openAiService)
        {
            _openAiService = openAiService;
        }

        [HttpPost(nameof(Create))]
        public async Task<string> Create([FromForm] string message)
        {
            var imageResult = await _openAiService.Image.CreateImage(
                new ImageCreateRequest
                {
                    //字符串 必填 所需图像的文本描述。最大长度为 1000 个字符
                    Prompt = message,
                    //整数或空 自选 默认值为 1 要生成的图像数。必须介于 1 和 10 之间
                    N = 1,
                    //字符串或空值  自选 默认值为 1024x1024 生成的图像的大小。必须是 256x256、512x512、1024x1024 之一
                    Size = StaticValues.ImageStatics.Size.Size256,
                    //字符串或空值 自选 默认为 url,必须是 url b64_json  之一
                    ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                    //字符串 自选 代表最终用户的唯一标识符，可帮助 OpenAI 监控和检测滥用行为
                    //User = "TestUser"
                });

            if (!imageResult.Successful)
                return $"{imageResult.Error.Code}: {imageResult.Error.Message}";

            return imageResult.Results.Select(r => r.Url).First();
        }


        [HttpPost(nameof(Edit))]
        public async Task<string> Edit([FromForm] string message, [FromForm] IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var imageResult = await _openAiService.Image.CreateImageEdit(
                new ImageEditCreateRequest
                {
                    //字符串 必填 要编辑的图像。必须是有效的 PNG 文件，小于 4MB，并且是正方形。如果未提供蒙版，则图像必须具有透明度，该透明度将用作蒙版。
                    Image = memoryStream.ToArray(),
                    ImageName = file.Name,
                    //字符串 必填 所需图像的文本描述。最大长度为 1000 个字符
                    Prompt = message,
                    //整数或空 自选 默认值为 1 要生成的图像数。必须介于 1 和 10 之间
                    N = 1,
                    //字符串或空值  自选 默认值为 1024x1024 生成的图像的大小。必须是 256x256、512x512、1024x1024 之一
                    Size = StaticValues.ImageStatics.Size.Size256,
                    //字符串或空值 自选 默认为 url,必须是 url b64_json  之一
                    ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                    //字符串 自选 代表最终用户的唯一标识符，可帮助 OpenAI 监控和检测滥用行为
                    //User = "TestUser"
                });

            if (!imageResult.Successful)
                return $"{imageResult.Error.Code}: {imageResult.Error.Message}";

            return imageResult.Results.Select(r => r.Url).First();
        }


        [HttpPost(nameof(Variation))]
        public async Task<string> Variation([FromForm] IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var imageResult = await _openAiService.Image.CreateImageVariation(
                new ImageVariationCreateRequest
                {
                    //字符串 必填 要编辑的图像。必须是有效的 PNG 文件，小于 4MB，并且是正方形。如果未提供蒙版，则图像必须具有透明度，该透明度将用作蒙版。
                    Image = memoryStream.ToArray(),
                    ImageName = file.Name,
                    //整数或空 自选 默认值为 1 要生成的图像数。必须介于 1 和 10 之间
                    N = 1,
                    //字符串或空值  自选 默认值为 1024x1024 生成的图像的大小。必须是 256x256、512x512、1024x1024 之一
                    Size = StaticValues.ImageStatics.Size.Size256,
                    //字符串或空值 自选 默认为 url,必须是 url b64_json  之一
                    ResponseFormat = StaticValues.ImageStatics.ResponseFormat.Url,
                    //字符串 自选 代表最终用户的唯一标识符，可帮助 OpenAI 监控和检测滥用行为
                    //User = "TestUser"
                });

            if (!imageResult.Successful)
                return $"{imageResult.Error.Code}: {imageResult.Error.Message}";

            return imageResult.Results.Select(r => r.Url).First();
        }

    }
}
