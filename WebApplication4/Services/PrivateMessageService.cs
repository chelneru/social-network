using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using WebApplication4.Models;

namespace WebApplication4.Services
{
    public class PrivateMessageService : BaseService
    {
        public PrivateMessageService()
        {

        }

        public bool CreatePrivateMessage(Guid initiatorUserProfileId, Guid TargetUserProfileId, string message)
        {
            var entity = new PrivateMessage
            {
                InitiatorUserProfileId = initiatorUserProfileId,
                TargetUserProfileId = TargetUserProfileId,
                Content = message,
                TimeStamp = DateTime.Now,
                Seen = false

            };
            try
            {
                Context.PrivateMessage.Add(entity);
                Context.SaveChanges();
                return true;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    System.Diagnostics.Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return false;

                throw;
            }
        }

        public  bool MarkPrivateMessageAsSeen(Guid pmId)
        {
            var entity = Context.PrivateMessage.FirstOrDefault(pm => pm.Id == pmId);
            if(entity != null)
            {
                entity.Seen = true;
                Context.SaveChanges();
            }
            return true;
        }

        public  List<PrivateMessage> GetAllPrivateMessages(Guid userProfileId1, Guid userProfileId2)
        {
            var result = Context.PrivateMessage
                .Where(pm => (pm.InitiatorUserProfileId == userProfileId1 && pm.TargetUserProfileId == userProfileId2) ||
            (pm.InitiatorUserProfileId == userProfileId2 && pm.TargetUserProfileId == userProfileId1)).AsNoTracking()
            .OrderBy(n => n.TimeStamp)
                .ToList();
            return result;
        }
    }
}