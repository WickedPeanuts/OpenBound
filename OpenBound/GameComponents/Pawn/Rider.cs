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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Asset;
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Pawn.Unit;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Sync;
using OpenBound_Network_Object_Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenBound.GameComponents.Pawn
{
    public enum AvatarState
    {
        Normal,
        Staring,
    }

    public class Rider
    {
        public Vector2 headBasePosition, bodyBasePosition,
            gogglesBasePosition, flagBasePosition, petBasePosition,
            extraBasePosition, miscBasePosition;

        public Avatar Head;
        public Avatar Body;
        public Avatar Goggles;
        public Avatar Flag;
        public Avatar Pet;

        public Avatar Extra;
        public Avatar Misc;

        readonly Mobile mobile;
        readonly List<int[]> riderOffset;

        //Rendering
        public bool ShouldRenderExtraAvatars;

#if DEBUG
        DebugCrosshair dc1 = new DebugCrosshair(Color.Blue);
        DebugCrosshair dc2 = new DebugCrosshair(Color.White);
#endif

        //Used on Avatar shop. No updates are supported for variables instanced with this constructor.
        public Rider(Facing facing, Player player, Vector2 position)
        {
            Head = new Avatar(MetadataManager.AvatarMetadataDictionary[player.Gender][AvatarCategory.Hat][player.EquippedAvatarHat]);
            Body = new Avatar(MetadataManager.AvatarMetadataDictionary[player.Gender][AvatarCategory.Body][player.EquippedAvatarBody]);

            Goggles = new Avatar(MetadataManager.AvatarMetadataDictionary[player.Gender][AvatarCategory.Goggles][player.EquippedAvatarGoggles], true);
            Flag = new Avatar(MetadataManager.AvatarMetadataDictionary[player.Gender][AvatarCategory.Flag][player.EquippedAvatarFlag], true);

            Pet = new Avatar(MetadataManager.AvatarMetadataDictionary[player.Gender][AvatarCategory.Pet][player.EquippedAvatarPet], true);
            Extra = new Avatar(MetadataManager.AvatarMetadataDictionary[player.Gender][AvatarCategory.Extra][player.EquippedAvatarExtra], true);
            Misc = new Avatar(MetadataManager.AvatarMetadataDictionary[player.Gender][AvatarCategory.Misc][player.EquippedAvatarMisc], true);

            int facingFactor = (facing == Facing.Right) ? -1 : 1;

            Head.Position = position + new Vector2(facingFactor * 7, -17);
            Body.Position = position + Vector2.Zero;
            Goggles.Position = position + new Vector2(facingFactor * 9, -17);
            Pet.Position = position + new Vector2(facingFactor * 7, -8);
            Extra.Position = position + new Vector2(0, -7);
            Misc.Position = position + new Vector2(0, -7);
            Flag.Position = position + new Vector2(facingFactor * 11, -17);

            if (facing == Facing.Right) Flip();

            ShouldRenderExtraAvatars = true;
        }

        public Rider(Mobile mobile)
        {
            this.mobile = mobile;

            Head = new Avatar(MetadataManager.AvatarMetadataDictionary[mobile.Owner.Gender][AvatarCategory.Hat][mobile.Owner.EquippedAvatarHat]);
            Body = new Avatar(MetadataManager.AvatarMetadataDictionary[mobile.Owner.Gender][AvatarCategory.Body][mobile.Owner.EquippedAvatarBody]);

            Goggles = new Avatar(MetadataManager.AvatarMetadataDictionary[mobile.Owner.Gender][AvatarCategory.Goggles][mobile.Owner.EquippedAvatarGoggles], true);
            Flag = new Avatar(MetadataManager.AvatarMetadataDictionary[mobile.Owner.Gender][AvatarCategory.Flag][mobile.Owner.EquippedAvatarFlag], true);

            Pet = new Avatar(MetadataManager.AvatarMetadataDictionary[mobile.Owner.Gender][AvatarCategory.Pet][mobile.Owner.EquippedAvatarPet], true);
            Extra = new Avatar(MetadataManager.AvatarMetadataDictionary[mobile.Owner.Gender][AvatarCategory.Extra][mobile.Owner.EquippedAvatarExtra], true);
            Misc = new Avatar(MetadataManager.AvatarMetadataDictionary[mobile.Owner.Gender][AvatarCategory.Misc][mobile.Owner.EquippedAvatarMisc], true);

            headBasePosition = new Vector2(7, -17);
            bodyBasePosition = Vector2.Zero;
            gogglesBasePosition = new Vector2(9, -17);
            flagBasePosition = new Vector2(11, -17);
            petBasePosition = new Vector2(7, -8);
            extraBasePosition = new Vector2(0, -7);
            miscBasePosition = new Vector2(0, -7);

            riderOffset = (List<int[]>)MetadataManager.ElementMetadata[$@"Mobile/{mobile.MobileType}/RiderPivot"];

#if DEBUG
            DebugHandler.Instance.Add(dc1);
            DebugHandler.Instance.Add(dc2);
#endif

            Update();

            ShouldRenderExtraAvatars = true;
        }

        public void Show()
        {
            Head.Show();
            Body.Show();

            Goggles.Show();
            Flag.Show();

            Pet.Show();

            Extra.Show();
            Misc.Show();
        }

        public void Hide()
        {
            Head.Hide();
            Body.Hide();

            Goggles.Hide();
            Flag.Hide();

            Pet.Hide();

            Extra.Hide();
            Misc.Hide();
        }

        public void Flip()
        {
            Head.Flip();
            Body.Flip();

            Goggles.Flip();
            Flag.Flip();

            Pet.Flip();

            Extra.Flip();
            Misc.Flip();
        }

        public int GetEquippedAvatarID(AvatarCategory avatarCategory)
        {
            switch (avatarCategory)
            {
                case AvatarCategory.Hat:      return Head.Metadata.ID;
                case AvatarCategory.Body:     return Body.Metadata.ID;
                case AvatarCategory.Goggles:  return Goggles.Metadata.ID;
                case AvatarCategory.Flag:     return Flag.Metadata.ID;
                //case AvatarCategory.ExItem: return EquippedAvatarExItem;
                case AvatarCategory.Pet:      return Pet.Metadata.ID;
                case AvatarCategory.Misc:     return Misc.Metadata.ID;
                default: return Extra.Metadata.ID;
            }
        }

        public void ReplaceAvatar(AvatarMetadata avatarMetadata)
        {
            Avatar avatar = new Avatar(avatarMetadata, 
                avatarMetadata.AvatarCategory != AvatarCategory.Hat &&
                avatarMetadata.AvatarCategory != AvatarCategory.Body);

            Avatar previousAvatar = null;

            switch (avatarMetadata.AvatarCategory)
            {
                case AvatarCategory.Hat:
                    previousAvatar = Head;
                    Head = avatar;
                    break;
                case AvatarCategory.Body:
                    previousAvatar = Body;
                    Body = avatar;
                    break;
                case AvatarCategory.Goggles:
                    previousAvatar = Goggles;
                    Goggles = avatar;
                    break;
                case AvatarCategory.Flag:
                    previousAvatar = Flag;
                    Flag = avatar;
                    break;
                case AvatarCategory.Pet:
                    previousAvatar = Pet;
                    Pet = avatar;
                    break;
                case AvatarCategory.Extra:
                    if (Extra == null) return;
                    previousAvatar = Extra;
                    Extra = avatar;
                    break;
                case AvatarCategory.Misc:
                    if (Misc == null) return;
                    previousAvatar = Misc;
                    Misc = avatar;
                    break;
            }

            avatar.Position = previousAvatar.Position;

            if (avatar.Flipbook.Effect != previousAvatar.Flipbook.Effect)
                avatar.Flip();

            ResetCurrentAnimation();
        }

        public void Update()
        {
            float baseAngle = mobile.MobileFlipbook.Rotation;
            Vector2 basePosition = Vector2.One;

            if (mobile.Facing == Facing.Right)
                basePosition = new Vector2(-1, 1);

            int value = mobile.MobileFlipbook.GetCurrentFrame();

            Matrix transform = Matrix.CreateRotationZ(baseAngle);

            //
            // Each mobile contains different RiderOffsets.
            // Exporting them with GunboundImageFix returns the
            // right coordinates but they need to be "centralized"
            // on the right position, for that, a fixed offset must
            // be added on each element of the list.
            //
            // For that, I am running this small script ONCE on every
            // RiderOffset export to fix the base coordinates and
            // replace 'em inside the json file.
            //
            // var x = RiderOffset.json
            // var y = "";
            //
            // x.forEach(myFunction);
            //
            // function myFunction(item, index)
            // {
            //   y += ("[ " + (item[0] + offsetX) + ", " + (item[1] + offsetY) + " ],\n");
            // }
            //
            // This comment will be deleted after I have confirmed
            // that all coordinates are working properly
            //

            Vector2 basePos = new Vector2(riderOffset[value][0], riderOffset[value][1]);
            Vector2 headPos = Vector2.Transform((basePos + headBasePosition) * basePosition, transform);
            Vector2 bodyPos = Vector2.Transform((basePos + bodyBasePosition) * basePosition, transform);
            Vector2 gogglesPos = Vector2.Transform((basePos + gogglesBasePosition) * basePosition, transform);
            Vector2 flagPos = Vector2.Transform((basePos + flagBasePosition) * basePosition, transform);
            Vector2 petPos = Vector2.Transform((basePos + petBasePosition) * basePosition, transform);
            

#if DEBUG
            dc1.Update(mobile.MobileFlipbook.Position + headPos);
            dc2.Update(mobile.MobileFlipbook.Position + bodyPos);
#endif

            Head.Position = mobile.MobileFlipbook.Position + headPos;
            Head.Rotation = mobile.MobileFlipbook.Rotation;
            Body.Position = mobile.MobileFlipbook.Position + bodyPos;
            Body.Rotation = mobile.MobileFlipbook.Rotation;

            Goggles.Position = mobile.MobileFlipbook.Position + gogglesPos;
            Goggles.Rotation = mobile.MobileFlipbook.Rotation;

            Flag.Position = mobile.MobileFlipbook.Position + flagPos;
            Flag.Rotation = mobile.MobileFlipbook.Rotation;

            Pet.Position = mobile.MobileFlipbook.Position + petPos;
            Pet.Rotation = mobile.MobileFlipbook.Rotation;

            Extra.Position = mobile.MobileFlipbook.Position + extraBasePosition;
            Misc.Position = mobile.MobileFlipbook.Position + miscBasePosition;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Head.Draw(gameTime, spriteBatch);
            Body.Draw(gameTime, spriteBatch);

            Goggles.Draw(gameTime, spriteBatch);
            Flag.Draw(gameTime, spriteBatch);
            Pet.Draw(gameTime, spriteBatch);

            if (ShouldRenderExtraAvatars)
            {
                Extra.Draw(gameTime, spriteBatch);
                Misc.Draw(gameTime, spriteBatch);
            }
        }

        internal void ResetCurrentAnimation()
        {
            Head.Flipbook.ResetCurrentAnimation();
            Body.Flipbook.ResetCurrentAnimation();

            Goggles.Flipbook.ResetCurrentAnimation();
            Flag.Flipbook.ResetCurrentAnimation();
            Pet.Flipbook.ResetCurrentAnimation();

            Extra.Flipbook.ResetCurrentAnimation();
            Misc.Flipbook.ResetCurrentAnimation();
        }
    }
}
