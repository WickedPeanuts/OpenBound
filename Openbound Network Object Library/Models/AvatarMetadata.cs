/* 
 * Copyright (C) 2020, Carlos H.M.S. <carlos_judo@hotmail.com>
 * This file is part of OpenBound.
 * OpenBound is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of the License, or(at your option) any later version.
 * 
 * OpenBound is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with OpenBound. If not, see http://www.gnu.org/licenses/.
 */

using Newtonsoft.Json;
using OpenBound_Network_Object_Library.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenBound_Network_Object_Library.Models
{
    public class AvatarMetadata
    {
        [Key, Column(Order = 1)] public int ID { get; set; }
        [Key, Column(Order = 2)] public Gender Gender { get; set; }
        [Key, Column(Order = 3)] public AvatarCategory AvatarCategory { get; set; }
        [Required] public string Name { get; set; }
        [Required] public DateTime Date { get; set; }
        [Required] public int GoldPrice { get; set; }
        [Required] public int CashPrice { get; set; }
        [Required] public int TokenPrice { get; set; }
        [Required] public float PivotX { get; set; }
        [Required] public float PivotY { get; set; }
        [Required] public int FrameDimensionX { get; set; }
        [Required] public int FrameDimensionY { get; set; }

        [JsonIgnore] public virtual ICollection<PlayerAvatarMetadata> PlayerAvatarMetadata { get; set; }

        [JsonIgnore, NotMapped]
        public float[] Pivot
        {
            get => new float[] { PivotX, PivotY };
            set
            {
                PivotX = value[0];
                PivotY = value[1];
            }
        }

        [JsonIgnore, NotMapped]
        public int[] FrameDimensions
        {
            get => new int[] { FrameDimensionX, FrameDimensionY };
            set
            {
                FrameDimensionX = value[0];
                FrameDimensionY = value[1];
            }
        }

        public AvatarMetadata() { }

        public AvatarMetadata(int partID, string name, DateTime date, AvatarCategory avatarCategory, Gender gender, int goldPrice, int cashPrice, int tokenPrice, float[] pivot, int[] frameDimensions)
        {
            ID = partID;
            Name = name;
            Date = date;
            AvatarCategory = avatarCategory;
            Gender = gender;
            GoldPrice = goldPrice;
            CashPrice = cashPrice;
            TokenPrice = tokenPrice;
            Pivot = pivot;
            FrameDimensions = frameDimensions;
        }

        public override int GetHashCode()
        {
            return ID ^ (int)AvatarCategory;
        }
    }
}
