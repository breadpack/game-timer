using System;
using System.Text.RegularExpressions;

namespace BreadPack.GameTimer.Utility {
    public class TimeOnly {
        public TimeOnly(int hour = 0, int minute = 0, int second = 0, int millisecond = 0) {
            Hour        = hour;
            Minute      = minute;
            Second      = second;
            Millisecond = millisecond;
        }
        public TimeOnly(DateTime dateTime) {
            Hour        = dateTime.Hour;
            Minute      = dateTime.Minute;
            Second      = dateTime.Second;
            Millisecond = dateTime.Millisecond;
        }
        public TimeOnly(TimeSpan timeSpan) {
            Hour        = timeSpan.Hours;
            Minute      = timeSpan.Minutes;
            Second      = timeSpan.Seconds;
            Millisecond = timeSpan.Milliseconds;
        }

        public int Hour {
            get => _hour;
            private set {
                if (value > 23 || value < 0)
                    throw new System.ArgumentOutOfRangeException(nameof(value), "hour must be between 0 and 23");
                _hour = value;
            }
        }

        public int Minute {
            get => _minute;
            private set {
                if (value > 59 || value < 0)
                    throw new System.ArgumentOutOfRangeException(nameof(value), "minute must be between 0 and 59");
                _minute = value;
            }
        }

        public int Second {
            get => _second;
            private set {
                if (value > 59 || value < 0)
                    throw new System.ArgumentOutOfRangeException(nameof(value), "second must be between 0 and 59");
                _second = value;
            }
        }

        public int Millisecond {
            get => _millisecond;
            private set {
                if (value > 999 || value < 0)
                    throw new System.ArgumentOutOfRangeException(nameof(value), "millisecond must be between 0 and 999");
                _millisecond = value;
            }
        }

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
        
        private static   Regex regex = new Regex(@"^(?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2})\.(?<milisecond>\d{3})$");
        private int   _hour;
        private int   _minute;
        private int   _second;
        private int   _millisecond;

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