using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mychelin.Models;
using Newtonsoft.Json;

using System.Diagnostics;

namespace Mychelin.Controllers
{
    public class HomeController : Controller
    {
        // �G���[���O�̎擾
        private readonly ILogger<HomeController> _logger;

        // �R���X�g���N�^�[
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Home/Index��Get���\�b�h
        public IActionResult Index()
        {
            // �Z�b�V��������Z�b�V����ID���擾
            var sessionId = HttpContext.Session.GetString("SessionId");

            // �Z�b�V����ID�����݂��Ȃ��ꍇ��Cookie���`�F�b�N
            if (sessionId == null)
            {
                HttpContext.Request.Cookies.TryGetValue("SessionId", out sessionId);
            }

            Person user = null;

            // �Z�b�V����ID���擾�ł����ꍇ�AJson��ϊ����ă��[�U�[���擾
            if (sessionId != null)
            {
                var userJson = HttpContext.Session.GetString(sessionId);
                if (userJson != null)
                {
                    user = JsonConvert.DeserializeObject<Person>(userJson);
                }
            }

            // �����Z�b�V�����������̓N�b�L�[������΃}�C�y�[�W��
            if (user != null)
            {
                return RedirectToAction("Index", "Shoplists");
            }
            // �Z�b�V�����������̓N�b�L�[���Ȃ����Home��\��
            else
            {
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
