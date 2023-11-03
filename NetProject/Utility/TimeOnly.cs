using System;
using System.Text.RegularExpressions;

namespace GameTimer.Utility {
    public class TimeOnly {
        public TimeOnly(int hour = 0, int minute = 0, int second = 0, int millisecond = 0) {
            if (hour > 23 || hour < 0)
                throw new System.ArgumentOutOfRangeException(nameof(hour), "hour must be between 0 and 23");
            if (minute > 59 || minute < 0)
                throw new System.ArgumentOutOfRangeException(nameof(minute), "minute must be between 0 and 59");
            if (second > 59 || second < 0)
                throw new System.ArgumentOutOfRangeException(nameof(second), "second must be between 0 and 59");
            if (millisecond > 999 || millisecond < 0)
                throw new System.ArgumentOutOfRangeException(nameof(millisecond), "millisecond must be between 0 and 999");
                    
            Hour = hour;
            Minute = minute;
            Second = second;
            Millisecond = millisecond;
        }
        public int Hour { get; }
        public int Minute { get; }
        public int Second { get; }
        public int Millisecond { get; }

        public override string ToString() {
            return $"{Hour:00}:{Minute:00}:{Second:00}.{Millisecond:000}";
        }
        public override bool Equals(object obj) {
            if (obj is TimeOnly timeOnly) {
                return Hour == timeOnly.Hour && Minute == timeOnly.Minute && Second == timeOnly.Second && Millisecond == timeOnly.Millisecond;
            }
            return false;
        }
        public override int GetHashCode() {
            return Hour.GetHashCode() ^ Minute.GetHashCode() ^ Second.GetHashCode() ^ Millisecond.GetHashCode();
        }
        
        private static Regex regex = new Regex(@"^(?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2})\.(?<milisecond>\d{3})$");
        public static bool TryParse(string s, out TimeOnly result) {
            result = null;
            var match = regex.Match(s);
            if (!match.Success) return false;
            var hour = int.Parse(match.Groups["hour"].Value);
            var minute = int.Parse(match.Groups["minute"].Value);
            var second = int.Parse(match.Groups["second"].Value);
            var millisecond = int.Parse(match.Groups["milisecond"].Value);
            result = new TimeOnly(hour, minute, second, millisecond);
            return true;
        }
        public static bool operator ==(TimeOnly left, TimeOnly right) {
            return left.Hour == right.Hour && left.Minute == right.Minute && left.Second == right.Second && left.Millisecond == right.Millisecond;
        }
        public static bool operator !=(TimeOnly left, TimeOnly right) {
            return !(left == right);
        }
        public static bool operator <(TimeOnly left, TimeOnly right) {
            if(left.Hour != right.Hour) return left.Hour < right.Hour;
            if(left.Minute != right.Minute) return left.Minute < right.Minute;
            if(left.Second != right.Second) return left.Second < right.Second;
            return left.Millisecond < right.Millisecond;
        }
        public static bool operator >(TimeOnly left, TimeOnly right) {
            if(left.Hour != right.Hour) return left.Hour > right.Hour;
            if(left.Minute != right.Minute) return left.Minute > right.Minute;
            if(left.Second != right.Second) return left.Second > right.Second;
            return left.Millisecond > right.Millisecond;
        }
        public static bool operator <=(TimeOnly left, TimeOnly right) {
            return !(left > right);
        }
        public static bool operator >=(TimeOnly left, TimeOnly right) {
            return !(left < right);
        }
        public static DateTime operator +(DateTime left, TimeOnly right) {
            return left.Date + new TimeSpan(0, right.Hour, right.Minute, right.Second, right.Millisecond);
        } 
    }
}