using Newtonsoft.Json;
using OpenBound_Network_Object_Library.Entity.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound_Network_Object_Library.Entity
{
    public class ProjectileDamageInformation
    {
        [JsonProperty("TI")] public int TargetID;
        [JsonProperty("TD")] public int TargetDamage;
        [JsonProperty("SA")] public int ShotAngle;
        [JsonProperty("TT")] public float TotalTravelledTime;
        [JsonProperty("WT")] public bool WasTargetKilled;
        [JsonProperty("SP")] public int[] StartingPosition;
        [JsonProperty("FP")] public int[] FinalPosition;
        [JsonProperty("SI")] public Facing ShooterInitialFacingPosition;
        [JsonProperty("IW")] public HashSet<WeatherType> InteractedWeatherSet;
    }

    public class ProjectileDamage
    {
        [JsonProperty("ID")] public int OwnerID;
        [JsonProperty("PD")] public List<ProjectileDamageInformation> ProjectileDamageInformationList;

        public ProjectileDamage(int ownerID)
        {
            OwnerID = ownerID;
            ProjectileDamageInformationList = new List<ProjectileDamageInformation>();
        }

        public void AddEntry(int id, int damage, int shotAngle, float totalTravelledSeconds,
            bool wasTargetKilled, int[] startingPosition, int[] finalPosition,
            Facing shooterInitialFacingPosition, HashSet<WeatherType> interactedWeatherList)
        {
            ProjectileDamageInformation pdi = ProjectileDamageInformationList.Find((x) => x.TargetID == id);

            if (pdi == null)
            {
                ProjectileDamageInformationList.Add(
                    new ProjectileDamageInformation()
                    {
                        TargetID = id,
                        TargetDamage = damage,
                        ShotAngle = shotAngle,
                        TotalTravelledTime = totalTravelledSeconds,
                        WasTargetKilled = wasTargetKilled,
                        StartingPosition = startingPosition,
                        FinalPosition = finalPosition,
                        ShooterInitialFacingPosition = shooterInitialFacingPosition,
                        InteractedWeatherSet = interactedWeatherList,
                    });
            }
            else
            {
                pdi.TargetDamage += damage;
                pdi.ShotAngle = Math.Max(pdi.ShotAngle, shotAngle);
                pdi.TotalTravelledTime = Math.Max(pdi.TotalTravelledTime, totalTravelledSeconds);
                pdi.WasTargetKilled |= wasTargetKilled;

                foreach(WeatherType wt in interactedWeatherList)
                    pdi.InteractedWeatherSet.Add(wt);
            }
        }
    }
}
