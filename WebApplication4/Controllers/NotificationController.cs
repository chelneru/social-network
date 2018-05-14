using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication4.Services;
using WebApplication4.SignalIR;

namespace WebApplication4.Controllers
{
    public class NotificationController : Controller
    {
        private readonly NotificationService  _notificationService= new NotificationService();
        private readonly UserProfileService  _userProfileService= new UserProfileService();
        
        [Route("notifications/", Name = "notifications")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var currentUserProfile = _userProfileService.GetUserProfileByUserId(new Guid(userId));
            var notifications = _notificationService.GetAllNotifications(currentUserProfile.Id);
            return View(notifications);
        }

        [Route("notifications/delete", Name = "notification-delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(Int16 notificationId)
        {
            _notificationService.DeleteNotification(notificationId);
            return Json(new { Message = "notification_deleted" });
        }
        [Route("notifications/delete-all", Name = "notification-delete-all")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteAll(Guid userProfileId)
        {
            _notificationService.DeleteAllNotifications(userProfileId);
            return Json(new { Message = "all_notification_deleted" });
        }
    }
}
