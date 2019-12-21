using System;
using SeventhServices.Resource.Common.Crypts;

namespace SeventhServices.Resource.Common.Classes
{
    public class Account
    {
        public string Pid { get; set; }

        public string Uuid { get; set; }

        public string EncPid { get; set; }

        public string Id { get; set; }

        public string Tpid { get; set; }

        public string Ivs { get; set; }

        public string EncUuid { get; set; }

        public Account()
        {
            
        }

        public Account(string pid, string uuid, bool isSaveEncrypt = true)
        {
            if (isSaveEncrypt)
            {
                Pid = AccountCrypt.Decrypt(pid);
                Uuid = AccountCrypt.Decrypt(uuid);
                EncPid = pid;
                Id = uuid;
                Tpid = Pid;
            }
            else
            {
                Pid = pid;
                Uuid = uuid;
                EncPid = AccountCrypt.Encrypt(pid);
                Id = AccountCrypt.Encrypt(uuid);
                Tpid = Pid;
            }
        }

        public Account(string tpid, string encUuid, string ivs)
        {
            Ivs = ivs;
            EncUuid = encUuid;
            Uuid = AccountCrypt.DecryptUuid(encUuid, ivs);
            Tpid = tpid;
            Pid = Tpid;
            EncPid = AccountCrypt.Encrypt(Pid);
            Id = AccountCrypt.Encrypt(Uuid);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetHashCode() != obj.GetHashCode())
            {
                return false;
            }

            return obj is Account account
                   && Pid == account.Pid
                   && Uuid == account.Uuid
                   && EncPid == account.EncPid
                   && Tpid == account.Tpid
                   && Id == account.Id
                   && Ivs == account.Ivs;
        }


        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Pid != null ? Pid.GetHashCode(StringComparison.Ordinal) : 0;
                hashCode = (hashCode * 397) ^ (Uuid != null ? Uuid.GetHashCode(StringComparison.Ordinal) : 0);
                hashCode = (hashCode * 397) ^ (EncPid != null ? EncPid.GetHashCode(StringComparison.Ordinal) : 0);
                hashCode = (hashCode * 397) ^ (Id != null ? Id.GetHashCode(StringComparison.Ordinal) : 0);
                hashCode = (hashCode * 397) ^ (Tpid != null ? Tpid.GetHashCode(StringComparison.Ordinal) : 0);
                hashCode = (hashCode * 397) ^ (Ivs != null ? Ivs.GetHashCode(StringComparison.Ordinal) : 0);
                hashCode = (hashCode * 397) ^ (EncUuid != null ? EncUuid.GetHashCode(StringComparison.Ordinal) : 0);
                return hashCode;
            }
        }
    }

}