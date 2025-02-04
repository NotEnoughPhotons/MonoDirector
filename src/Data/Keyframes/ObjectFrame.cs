using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace NEP.MonoDirector.Data
{
    public struct ObjectFrame : IBinaryData
    {
        //
        // Constructors
        //
        public ObjectFrame(Vector3 position)
        {
            name = "Bone";
            this.transform = null;
            this.position = position;
            this.rotation = Quaternion.identity;
            this.scale = Vector3.one;

            frameTime = 0f;

            rigidbody = null;
            rigidbodyVelocity = Vector3.zero;
            rigidbodyAngularVelocity = Vector3.zero;
            activeThisFrame = true;
        }

        public ObjectFrame(Vector3 position, Quaternion rotation)
        {
            name = "Bone";
            this.transform = null;
            this.position = position;
            this.rotation = rotation;
            this.scale = Vector3.one;

            frameTime = 0f;

            rigidbody = null;
            rigidbodyVelocity = Vector3.zero;
            rigidbodyAngularVelocity = Vector3.zero;
            activeThisFrame = true;
        }

        public ObjectFrame(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            name = "Bone";
            this.transform = null;
            this.position = position;
            this.rotation = rotation;
            this.scale = scale;

            frameTime = 0f;

            rigidbody = null;
            rigidbodyVelocity = Vector3.zero;
            rigidbodyAngularVelocity = Vector3.zero;
            activeThisFrame = true;
        }

        public ObjectFrame(Quaternion rotation)
        {
            name = "Bone";
            this.transform = null;
            this.position = Vector3.zero;
            this.rotation = rotation;
            this.scale = Vector3.one;

            frameTime = 0f;

            rigidbody = null;
            rigidbodyVelocity = Vector3.zero;
            rigidbodyAngularVelocity = Vector3.zero;
            activeThisFrame = true;
        }

        public ObjectFrame(Transform transform)
        {
            name = transform != null ? transform.name : "Null";

            this.transform = transform;
            position = transform != null ? transform.position : Vector3.zero;
            rotation = transform != null ? transform.rotation : Quaternion.identity;
            scale = transform != null ? transform.localScale : Vector3.one;

            frameTime = 0f;

            rigidbody = null;
            rigidbodyVelocity = Vector3.zero;
            rigidbodyAngularVelocity = Vector3.zero;
            activeThisFrame = transform.gameObject.activeSelf;
        }

        public ObjectFrame(Rigidbody rigidbody)
        {
            name = rigidbody.name;

            this.transform = rigidbody.transform;
            position = transform != null ? transform.position : Vector3.zero;
            rotation = transform != null ? transform.rotation : Quaternion.identity;
            scale = transform != null ? transform.localScale : Vector3.one;

            frameTime = 0f;

            this.rigidbody = rigidbody;
            rigidbodyVelocity = rigidbody.velocity;
            rigidbodyAngularVelocity = rigidbody.angularVelocity;
            activeThisFrame = rigidbody.gameObject.activeSelf;
        }

        public void SetDelta(float frameTime)
        {
            this.frameTime = frameTime;
        }
        
        //
        // Serialization data
        //
        public enum VersionNumber : short
        {
            V1
        }

        // V1 contains the following data
        // 
        // version: u16
        // position: v3
        // rotation: v4
        // frame_time: float
        //
        // 8 floats = sizeof(float) * 8
        // 1 short = sizeof(short)
        // Total = sizeof(float) * 8 + sizeof(short)

        public static Dictionary<VersionNumber, ushort> VersionSizes = new Dictionary<VersionNumber, ushort>()
        {
            { VersionNumber.V1, sizeof(float) * 8 + sizeof(short) }
        };

        //
        // Members
        //
        public string name;

        public Transform transform;
        public Rigidbody rigidbody;

        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public float frameTime;
        public bool activeThisFrame;

        public Vector3 rigidbodyVelocity;
        public Vector3 rigidbodyAngularVelocity;
        
        //
        // IBinaryData
        //
        public byte[] ToBinary()
        {
            // Writes a version 1 object frame
            byte[] bytes = new byte[VersionSizes[VersionNumber.V1]];

            Array.Copy(
                BitConverter.GetBytes((short)VersionNumber.V1), 
                0, 
                bytes, 
                0, 
                sizeof(short)
            );
            
            Array.Copy(
                BitConverter.GetBytes(position.x), 
                0, 
                bytes, 
                sizeof(short), 
                sizeof(float)
            );

            Array.Copy(
                BitConverter.GetBytes(position.y), 
                0, 
                bytes, 
                sizeof(short) + sizeof(float), 
                sizeof(float)
            );

            
            Array.Copy(
                BitConverter.GetBytes(position.z), 
                0, 
                bytes, 
                sizeof(short) + sizeof(float) * 2, 
                sizeof(float)
            );
            

            Array.Copy(
                BitConverter.GetBytes(rotation.x), 
                0, 
                bytes, 
                sizeof(short) + sizeof(float) * 3, 
                sizeof(float)
            );

            Array.Copy(
                BitConverter.GetBytes(rotation.y), 
                0, 
                bytes, 
                sizeof(short) + sizeof(float) * 4, 
                sizeof(float)
            );
            
            Array.Copy(
                BitConverter.GetBytes(rotation.z), 
                0, 
                bytes, 
                sizeof(short) + sizeof(float) * 5, 
                sizeof(float)
            );
            
            Array.Copy(
                BitConverter.GetBytes(rotation.w), 
                0, 
                bytes, 
                sizeof(short) + sizeof(float) * 6, 
                sizeof(float)
            );
            
            Array.Copy(
                BitConverter.GetBytes(frameTime), 
                0, 
                bytes, 
                sizeof(short) + sizeof(float) * 7, 
                sizeof(float)
            );
            
            return bytes;
        }

        public void FromBinary(Stream stream)
        {
            //Main.Logger.Msg($"[ObjectFrame]: FromBinary() Called!");
            
            // Check the version number
            byte[] versionBytes = new byte[sizeof(short)];
            stream.Read(versionBytes, 0, versionBytes.Length);
            
            short version = BitConverter.ToInt16(versionBytes, 0);

            if (version != (short)VersionNumber.V1)
                throw new Exception($"Unsupported version type! Value was {version}");

            // Copy the data back into our structures
            // This is dependent on the version number!
            if (version == (short)VersionNumber.V1)
            {
                byte[] bytes = new byte[sizeof(float) * 8];
                stream.Read(bytes, 0, bytes.Length);
                
                position = new Vector3(
                    BitConverter.ToSingle(bytes, 0),
                    BitConverter.ToSingle(bytes, sizeof(float)),
                    BitConverter.ToSingle(bytes, sizeof(float) * 2)
                );

                rotation = new Quaternion(
                    BitConverter.ToSingle(bytes, sizeof(float) * 3),
                    BitConverter.ToSingle(bytes, sizeof(float) * 4),
                    BitConverter.ToSingle(bytes, sizeof(float) * 5),
                    BitConverter.ToSingle(bytes, sizeof(float) * 6)
                );

                frameTime = BitConverter.ToSingle(bytes, sizeof(float) * 7);
            }
        }

        // The identifier is OBFD in ASCII
        // It's backwards bc endianness :(
        public uint GetBinaryID() => 0x4446424F;
    }
}
