using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace XIVSocket.App.Models
{
    public class SerializableVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public SerializableVector3() { }

        public SerializableVector3(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        public SerializableVector3(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(X, Y, Z);
        }

        // create an equals override to compare two SerializableVector3 objects
        public override bool Equals(object obj)
        {
            if(obj == null) {
                return false;
            }

            if(obj.GetType() == typeof(Vector3)) {
                Vector3 v3 = (Vector3)obj;
                return (X == v3.X) && (Y == v3.Y) && (Z == v3.Z);
            }

            if (GetType() != obj.GetType()) {
                return false;
            }

            SerializableVector3 v = (SerializableVector3)obj;
            return (X == v.X) && (Y == v.Y) && (Z == v.Z);
        }

        public override string ToString() {
            return new Vector3(X, Y, Z).ToString();
        }
    }
}
