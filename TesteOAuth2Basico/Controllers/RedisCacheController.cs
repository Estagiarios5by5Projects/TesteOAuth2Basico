//using Microsoft.AspNetCore.Mvc;
//using Services;

//namespace TesteOAuth2Basico.Controllers
//{
//    public class RedisCacheController : Controller
//    {
//        private readonly RedisCache _redisCache;

//        public RedisCacheController(RedisCache redisCache)
//        {
//            _redisCache = redisCache;
//        }

//        [HttpPost("set")]
//        public async Task<IActionResult> SetCache(string key, string value)
//        {
//            await _redisCache.SetStringAsync(key, value);
//            return Ok();
//        }

//        [HttpGet("get")]
//        public async Task<IActionResult> GetCache(string key)
//        {
//            var value = await _redisCache.GetStringAsync(key);
//            return Ok(value);
//        }
//    }
//}
