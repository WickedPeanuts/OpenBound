using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OpenBound_Network_Object_Library.Models
{
    public enum PlayerRelationshipStatus
    {
        Requested,
        Accepted,
        Blocked,
    }
    public class PlayerRelationship
    {
        [Key]
        public int ID { get; set; }

        public virtual Player Player { get; set; }

        public PlayerRelationshipStatus PlayerRelationshipStatus { get; set; }
    }
}
