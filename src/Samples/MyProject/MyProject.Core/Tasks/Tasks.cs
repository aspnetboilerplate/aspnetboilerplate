using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using MyProject.People;

namespace MyProject.Tasks
{
    /// <summary>
    /// Represents a Task entity.
    /// 
    /// Inherits from <see cref="Entity{TPrimaryKey}"/> class (Optionally can implement <see cref="IEntity{TPrimaryKey}"/> directly).
    /// 
    /// An Entity's primary key is assumed as <see cref="int"/> as default.
    /// If it's different, we must declare it as generic parameter (as done here for <see cref="long"/>).
    /// 
    /// Implements <see cref="IHasCreationTime"/>, thus ABP sets CreationTime automatically while saving to database.
    /// Also, this helps us to use standard naming and functionality for 'creation time' of entities.
    /// </summary>
    [Table("StsTasks")]
    public class Task : Entity<long>, IHasCreationTime
    {
        /// <summary>
        /// A reference (navigation property) to assigned <see cref="Person"/> for this task.
        /// We declare <see cref="ForeignKeyAttribute"/> for EntityFramework here. No need for NHibernate.
        /// </summary>
        [ForeignKey("AssignedPersonId")]
        public virtual Person AssignedPerson { get; set; }

        /// <summary>
        /// Database field for AssignedPerson reference.
        /// Needed for EntityFramework, no need for NHibernate.
        /// </summary>
        public virtual int? AssignedPersonId { get; set; }

        /// <summary>
        /// Describes the task.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// The time when this task is created.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Current state of the task.
        /// </summary>
        public virtual TaskState State { get; set; }

        /// <summary>
        /// Default costructor.
        /// Assigns some default values to properties.
        /// </summary>
        public Task()
        {
            CreationTime = DateTime.Now;
            State = TaskState.Active;
        }
    }
}
