using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace OpenBound_Network_Object_Library.Models
{
    public enum PaymentMethod {
        Free,
        Gold,
        Cash,
        Token
    }
    public class PlayerAvatarMetadata
    {
        public int Player_ID { get; set; }
        public virtual Player Player { get; set; }
        public int AvatarMetadata_ID { get; set; }
        public Gender AvatarMetadata_Gender { get; set; }
        public AvatarCategory AvatarMetadata_AvatarCategory { get; set; }
        public virtual AvatarMetadata AvatarMetadata { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime BoughtAt { get; set; } = DateTime.Now;

        public PlayerAvatarMetadata() { }

        public PlayerAvatarMetadata(Player player, AvatarMetadata avatarMetadata, PaymentMethod paymentMethod)
        {
            Player_ID = player.ID;

            AvatarMetadata_ID = avatarMetadata.ID;
            AvatarMetadata_Gender = avatarMetadata.Gender;
            AvatarMetadata_AvatarCategory = avatarMetadata.AvatarCategory;

            PaymentMethod = paymentMethod;
        }
    }
}
