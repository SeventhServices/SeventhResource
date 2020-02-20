using System;

namespace Seventh.Resource.Common.Classes
{
    public class GameVersion :IComparable<GameVersion>,IEquatable<GameVersion>
    {
        public string Version { get; set; } = "0";
        public string DownloadPath { get; set; }

        public int CompareTo(GameVersion other)
        {
            if (ReferenceEquals(this, other))
            {
                return 0;
            }

            if (other is null)
            {
                return 1;
            }

            return string.Compare(Version, other.Version, StringComparison.Ordinal);

        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((GameVersion) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Version != null ? Version.GetHashCode(StringComparison.CurrentCulture) : 0) * 397) ^ (DownloadPath != null ? DownloadPath.GetHashCode(StringComparison.CurrentCulture) : 0);
            }
        }

        public static bool operator ==(GameVersion left, GameVersion right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(GameVersion left, GameVersion right)
        {
            return !(left == right);
        }

        public static bool operator <(GameVersion left, GameVersion right)
        {
            return left is null ? !(right is null) : left.CompareTo(right) < 0;
        }

        public static bool operator <=(GameVersion left, GameVersion right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(GameVersion left, GameVersion right)
        {
            return !(left is null) && left.CompareTo(right) > 0;
        }

        public static bool operator >=(GameVersion left, GameVersion right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }

        public bool Equals(GameVersion other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Version == other.Version && DownloadPath == other.DownloadPath;
        }
    }
}
