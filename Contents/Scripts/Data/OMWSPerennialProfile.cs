using CryingOnion.OhMy.WeatherSystem.Utility;
using UnityEngine;

namespace CryingOnion.OhMy.WeatherSystem.Data
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "Crying Onion/Oh My Weather System/Perennial Profile", order = 361)]
    public class OMWSPerennialProfile : OMWSClimateProfile
    {
        [Tooltip("Specifies the current ticks.")]
        public float currentTicks;

        [Tooltip("Specifies the current day.")]
        public int currentDay;

        [Tooltip("Specifies the current year.")]
        public int currentYear;

        [HideInInspector]
        public float dayAndTime;

        public bool pauseTime;

        [Tooltip("Should this profile use a series of months for a more realistic year.")]
        public bool realisticYear;

        [Tooltip("Should this profile use a longer year every 4th year.")]
        public bool useLeapYear;

        [Tooltip("Should this system reset the ticks when it loads or should it pull the current ticks from the scriptable object?")]
        public bool resetTicksOnStart = false;

        [Tooltip("The ticks that this system should start at when the scene is loaded.")]
        public float startTicks = 120;

        [Tooltip("Specifies the maximum amount of ticks per day.")]
        public float ticksPerDay = 360;

        [Tooltip("Specifies the amount of ticks that passs in a second.")]
        public float tickSpeed = 1;

        [Tooltip("Changes tick speed based on the percentage of the day.")]
        public AnimationCurve tickSpeedMultiplier;

        [System.Serializable]
        public struct TimeWeightRelation
        {
            [Range(0, 1)] public float time; [Range(0, 360)] public float sunHeight; [Range(0, 1)] public float weight;

            public TimeWeightRelation(float time, float sunHeight, float weight)
            {
                this.time = time;
                this.sunHeight = sunHeight;
                this.weight = weight;
            }
        }

        [Tooltip("Specifies the amount of ticks that passs in a second.")]
        public TimeWeightRelation sunriseWeight = new TimeWeightRelation(0.25f, 90, 0.2f);

        [Tooltip("Specifies the amount of ticks that passs in a second.")]
        public TimeWeightRelation dayWeight = new TimeWeightRelation(0.5f, 180, 0.2f);

        [Tooltip("Specifies the amount of ticks that passs in a second.")]
        public TimeWeightRelation sunsetWeight = new TimeWeightRelation(0.75f, 270, 0.2f);

        [Tooltip("Specifies the amount of ticks that passs in a second.")]
        public TimeWeightRelation nightWeight = new TimeWeightRelation(1, 360, 0.2f);

        [HideInInspector]
        public AnimationCurve sunMovementCurve;

        [OMWSHideTitle]
        public AnimationCurve displayCurve;

        [System.Serializable]
        public class OMWSMonth
        {
            public string name;
            public int days;
        }

        [OMWSMonthList]
        public OMWSMonth[] standardYear = new OMWSMonth[12] { new OMWSMonth() { days = 31, name = "January"}, new OMWSMonth() { days = 28, name = "Febraury" },
        new OMWSMonth() { days = 31, name = "March"}, new OMWSMonth() { days = 30, name = "April"}, new OMWSMonth() { days = 31, name = "May"},
        new OMWSMonth() { days = 30, name = "June"}, new OMWSMonth() { days = 31, name = "July"}, new OMWSMonth() { days = 31, name = "August"},
        new OMWSMonth() { days = 30, name = "September"}, new OMWSMonth() { days = 31, name = "October"}, new OMWSMonth() { days = 30, name = "Novemeber"},
        new OMWSMonth() { days = 31, name = "December"}};

        [OMWSMonthList]
        public OMWSMonth[] leapYear = new OMWSMonth[12] { new OMWSMonth() { days = 31, name = "January"}, new OMWSMonth() { days = 29, name = "Febraury" },
        new OMWSMonth() { days = 31, name = "March"}, new OMWSMonth() { days = 30, name = "April"}, new OMWSMonth() { days = 31, name = "May"},
        new OMWSMonth() { days = 30, name = "June"}, new OMWSMonth() { days = 31, name = "July"}, new OMWSMonth() { days = 31, name = "August"},
        new OMWSMonth() { days = 30, name = "September"}, new OMWSMonth() { days = 31, name = "October"}, new OMWSMonth() { days = 30, name = "Novemeber"},
        new OMWSMonth() { days = 31, name = "December"}};

        public enum DefaultYear { January, February, March, April, May, June, July, August, September, October, November, December }
        public enum TimeDivisors { Early, Mid, Late }

        public enum TimeCurveSettings { linearDay, simpleCurve, advancedCurve }
        public TimeCurveSettings timeCurveSettings;

        public int daysPerYear = 48;

        public void GetModifiedDayPercent()
        {
            switch (timeCurveSettings)
            {
                case (TimeCurveSettings.advancedCurve):
                    sunMovementCurve = new AnimationCurve(new Keyframe[5]
                    {
                        new Keyframe(0, 0, 0, 0, nightWeight.weight, nightWeight.weight),
                        new Keyframe(sunriseWeight.time, sunriseWeight.sunHeight, 0, 0, sunriseWeight.weight, sunriseWeight.weight),
                        new Keyframe(dayWeight.time, dayWeight.sunHeight, 0, 0, dayWeight.weight, dayWeight.weight),
                        new Keyframe(sunsetWeight.time, sunsetWeight.sunHeight, 0, 0, sunsetWeight.weight, sunsetWeight.weight),
                        new Keyframe(1, sunsetWeight.sunHeight > dayWeight.sunHeight ? 360 : 0, 0, 0, nightWeight.weight, nightWeight.weight)
                    });

                    displayCurve = new AnimationCurve(new Keyframe[5]
                    {
                        new Keyframe(0, 0, 0, 0, nightWeight.weight, nightWeight.weight),
                        new Keyframe(sunriseWeight.time, sunriseWeight.sunHeight, 0, 0, sunriseWeight.weight, sunriseWeight.weight),
                        new Keyframe(dayWeight.time, dayWeight.sunHeight, 0, 0, dayWeight.weight, dayWeight.weight),
                        new Keyframe(sunsetWeight.time, sunsetWeight.sunHeight > 180 ? 360 - sunsetWeight.sunHeight : sunsetWeight.sunHeight, 0, 0, sunsetWeight.weight, sunsetWeight.weight),
                        new Keyframe(1, 0, 0, 0, nightWeight.weight, nightWeight.weight)
                    });
                    break;

                case (TimeCurveSettings.simpleCurve):
                    sunMovementCurve = new AnimationCurve(new Keyframe[5]
                    {
                        new Keyframe(0, 0, 0, 0, nightWeight.weight, nightWeight.weight),
                        new Keyframe(0.25f, 90f, 0, 0, sunriseWeight.weight, sunriseWeight.weight),
                        new Keyframe(0.5f, 180f, 0, 0, dayWeight.weight, dayWeight.weight),
                        new Keyframe(0.75f, 270f, 0, 0, sunsetWeight.weight, sunsetWeight.weight),
                        new Keyframe(1, 360, 0, 0, nightWeight.weight, nightWeight.weight)
                    });

                    displayCurve = new AnimationCurve(new Keyframe[5]
                    {
                        new Keyframe(0, 0, 0, 0, nightWeight.weight, nightWeight.weight),
                        new Keyframe(0.25f, 90f, 0, 0, sunriseWeight.weight, sunriseWeight.weight),
                        new Keyframe(0.5f, 180f, 0, 0, dayWeight.weight, dayWeight.weight),
                        new Keyframe(0.75f, 90, 0, 0, sunsetWeight.weight, sunsetWeight.weight),
                        new Keyframe(1, 0, 0, 0, nightWeight.weight, nightWeight.weight)
                    });
                    break;

                case (TimeCurveSettings.linearDay):
                    sunMovementCurve = new AnimationCurve(new Keyframe[5]
                    {
                        new Keyframe(0, 0, 0, 0, 0, 0),
                        new Keyframe(0.25f, 90, 0, 0, 0, 0),
                        new Keyframe(0.5f, 180, 0, 0, 0, 0),
                        new Keyframe(0.75f, 270, 0, 0, 0, 0),
                        new Keyframe(1, 360, 0, 0, 0, 0)
                    });

                    displayCurve = new AnimationCurve(new Keyframe[5]
                    {
                        new Keyframe(0, 0, 0, 0, 0, 0),
                        new Keyframe(0.25f, 90, 0, 0, 0, 0),
                        new Keyframe(0.5f, 180, 0, 0, 0, 0),
                        new Keyframe(0.75f, 90, 0, 0, 0, 0),
                        new Keyframe(1, 0, 0, 0, 0, 0)
                    });
                    break;
            }
        }


        /// <summary>
        /// Returns the formatted time at a certain tick value.  
        /// <param name="militaryTime">Should the time be formatted in military time (24 hour day)? </param>
        /// <param name="ticks">The number of ticks  </param>
        /// </summary> 
        public string FormatTime(bool militaryTime, float ticks)
        {
            float time = ticks / ticksPerDay;

            int minutes = Mathf.RoundToInt(time * 1440);
            int hours = (minutes - minutes % 60) / 60;
            minutes -= hours * 60;

            if (militaryTime)
                return "" + hours.ToString("D2") + ":" + minutes.ToString("D2");
            else if (hours == 0)
                return "" + 12 + ":" + minutes.ToString("D2") + "AM";
            else if (hours == 12)
                return "" + 12 + ":" + minutes.ToString("D2") + "PM";
            else if (hours > 12)
                return "" + (hours - 12) + ":" + minutes.ToString("D2") + "PM";
            else
                return "" + (hours) + ":" + minutes.ToString("D2") + "AM";
        }

        public float ModifiedTickSpeed() => tickSpeed * tickSpeedMultiplier.Evaluate(currentTicks / ticksPerDay);

        public int RealisticDaysPerYear()
        {
            int i = 0;
            foreach (OMWSMonth j in (useLeapYear && currentYear % 4 == 0) ? leapYear : standardYear) i += j.days;
            return i;
        }
    }
}