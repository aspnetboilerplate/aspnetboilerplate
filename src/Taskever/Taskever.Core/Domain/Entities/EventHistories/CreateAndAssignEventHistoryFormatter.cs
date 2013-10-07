using System;

namespace Taskever.Domain.Entities.EventHistories
{
    public class CreateAndAssignEventHistoryFormatter : EventHistoryFormatter
    {
        public override EventHistoryType Type { get { return EventHistoryType.CreateAndAssignTask; } }

        public override short LatestVersion { get { return 1; } }
        
        public int CreatorUserId { get; set; }
        
        public int TaskId { get; set; }
        
        public int AssignedUserId { get; set; }

        public CreateAndAssignEventHistoryFormatter()
        {
            
        }

        public CreateAndAssignEventHistoryFormatter(int creatorUserId, int taskId, int assignedUserId)
        {
            CreatorUserId = creatorUserId;
            TaskId = taskId;
            AssignedUserId = assignedUserId;
        }

        public override EventHistory CreateEventHistory()
        {
            var eventHistory = new EventHistory();
            WriteTo(eventHistory);
            return eventHistory;
        }

        public override void WriteTo(EventHistory history)
        {
            history.HistoryText = string.Join("|", new object[] { CreatorUserId, TaskId, AssignedUserId });
            history.HistoryType = Type;
            history.HistoryVersion = LatestVersion;
        }

        public override void ReadFrom(EventHistory history)
        {
            if (history.HistoryVersion != 1)
            {
                throw new NotSupportedException(); //TODO: Throw a better exception!
            }

            var splitted = history.HistoryText.Split('|');
            CreatorUserId = Convert.ToInt32(splitted[0]);
            TaskId = Convert.ToInt32(splitted[1]);
            AssignedUserId = Convert.ToInt32(splitted[2]);
        }
    }
}