using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataReader;

namespace SMSReader
{
    public class BckFile
    {
        byte[] tag;
        byte[] type;

        public List<BckSection> sections = new List<BckSection>();

        public BckFile(string path)
        {
            FileStream fs = null;
            tag = new byte[4];
            type = new byte[4];
            UInt32 size;
            UInt32 sectioncount;

            fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            fs.Read(tag, 0, 4);//Read the file tag at the start, j3dview

            if ((char)tag[0] != 'J' || (char)tag[1] != '3' || (char)tag[2] != 'D')//Check file tag
                throw new FileLoadException("Not a valid bck file.");

            fs.Read(type, 0, 4);//Read file type should be bck1

            if ((char)type[0] != 'b' || (char)type[1] != 'c' || (char)type[2] != 'k' || (char)type[3] != '1')//Check
                throw new FileLoadException("Not an animation file.");

            size = Data.ReadUInt32(fs);//Get size of file by reading it from the file header

            if (size != fs.Length)//Check if this matches the real file size
                Console.WriteLine("Bck: Inconsistent file size.");

            sectioncount = Data.ReadUInt32(fs);//Read the next 4 bytes as integer. This gives the total number of sections.

            fs.Seek(16, SeekOrigin.Current);//skip buffer

            for (int i = 0; i < sectioncount; i++)//Get sections
                sections.Add(BckSection.LoadSection(fs));
            fs.Close();
        }
    }

    public class AnimKeyFrame
    {
        public float time = 0.0f;
        public float value = 0.0f;
        public float tangent_in = 0.0f;
        public float tangent_out = 0.0f;
    }

    public class BckSection
    {
        string magic;//section tag, we want ank1
        UInt32 size;//total size of section(including header), in bytes

        public static BckSection LoadSection(Stream stream)
        {
            string section;//magic
            UInt32 secsize;//sectionsize

            section = Data.ReadString(stream, 4);
            secsize = Data.ReadUInt32(stream);

            if (stream.Length - stream.Position + 8 < secsize)
                Console.WriteLine("Bck: Inconsistent section size.");

            switch (section)
            {
                case "ANK1":
                    return new BckANK1(secsize, stream);
                default:
                    throw new FileLoadException("Unknown section type \"" + section + "\"");
            }
        }

        public class BckANK1 : BckSection
        {   //Got structure from blank's J3DView https://github.com/blank63/j3dview/blob/master/j3d/ank1.py
            public struct Header//header(not including magic or section size)
            {
                public byte loopmode;
                public byte anglescaleexp;
                public UInt16 duration;
                public UInt16 jointanimationcount;
                public UInt16 scalecount;
                public UInt16 rotcount;
                public UInt16 translationcount;
                public UInt32 jointanimationoffset;
                public UInt32 scaleoffset;
                public UInt32 rotoffset;
                public UInt32 translationoffset;

                public static Header Read(Stream str)//load header of section
                {
                    Header hdr = new Header();//
                    hdr.loopmode = Data.ReadByte(str);
                    hdr.anglescaleexp = Data.ReadByte(str);
                    hdr.duration = Data.ReadUInt16(str);
                    hdr.jointanimationcount = Data.ReadUInt16(str);
                    hdr.scalecount = Data.ReadUInt16(str);
                    hdr.rotcount = Data.ReadUInt16(str);
                    hdr.translationcount = Data.ReadUInt16(str);
                    hdr.jointanimationoffset = Data.ReadUInt32(str);
                    hdr.scaleoffset = Data.ReadUInt32(str);
                    hdr.rotoffset = Data.ReadUInt32(str);
                    hdr.translationoffset = Data.ReadUInt32(str);
                    return hdr;
                }
            }
            public struct Selection
            {
                public UInt16 count;
                public UInt16 first;
                public UInt16 u1;

                public static Selection Read(Stream str)
                {
                    Selection sel = new Selection();
                    sel.count = Data.ReadUInt16(str);
                    sel.first = Data.ReadUInt16(str);
                    sel.u1 = Data.ReadUInt16(str);
                    return sel;
                }
            }
            public struct ComponentAnimation
            {
                public Selection scale;
                public Selection rotation;
                public Selection translation;

                public static ComponentAnimation Read(Stream str)
                {
                    ComponentAnimation anim = new ComponentAnimation();
                    anim.scale = Selection.Read(str);
                    anim.rotation = Selection.Read(str);
                    anim.translation = Selection.Read(str);
                    return anim;
                }
            }
            public struct JointAnimation
            {
                public ComponentAnimation x;
                public ComponentAnimation y;
                public ComponentAnimation z;

                public static JointAnimation Read(Stream str)
                {
                    JointAnimation anim = new JointAnimation();
                    anim.x = ComponentAnimation.Read(str);
                    anim.y = ComponentAnimation.Read(str);
                    anim.z = ComponentAnimation.Read(str);
                    return anim;
                }
            }

            public class Animation
            {
                public enum InterpolationType
                {
                    Linear,
                    Cubic
                }

                public List<AnimKeyFrame> x;
                public List<AnimKeyFrame> y;
                public List<AnimKeyFrame> z;

                public List<AnimKeyFrame> rotationx;
                public List<AnimKeyFrame> rotationy;
                public List<AnimKeyFrame> rotationz;

                public List<AnimKeyFrame> scalex;
                public List<AnimKeyFrame> scaley;
                public List<AnimKeyFrame> scalez;

                public float Duration
                {
                    get
                    {
                        return Max(GetDurationOfKeyFrameList(x),         GetDurationOfKeyFrameList(y),         GetDurationOfKeyFrameList(z),
                                   GetDurationOfKeyFrameList(rotationx), GetDurationOfKeyFrameList(rotationy), GetDurationOfKeyFrameList(rotationz),
                                   GetDurationOfKeyFrameList(scalex),    GetDurationOfKeyFrameList(scaley),    GetDurationOfKeyFrameList(scalez));
                    }
                }

                private static float Max(params float[] values)
                {
                    return Enumerable.Max(values);
                }
                private static float GetDurationOfKeyFrameList(List<AnimKeyFrame> list)
                {
                    AnimKeyFrame prev = new AnimKeyFrame();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].time >= prev.time)
                            prev = list[i];
                    }
                    return prev.time;
                }

                private static float InterpolateKeyFrameList(List<AnimKeyFrame> list, float time, InterpolationType type = InterpolationType.Linear)
                {
                    AnimKeyFrame prev = null;
                    AnimKeyFrame after = null;

                    for (int i = 0; i < list.Count; i++)
                    {
                        if ((prev == null || list[i].time >= prev.time) && list[i].time <= time)
                            prev = list[i];
                        if ((after == null || list[i].time <= after.time) && list[i].time >= time)
                        {
                            after = list[i];
                            //We can't break here; the list isn't necessarily ordered by time.
                        }

                    }

                    if (after == null && prev == null)//this shouldn't happen
                        after = prev = new AnimKeyFrame();
                    else if (after == null)
                        after = prev;
                    else if (prev == null)
                        prev = after;

                    switch (type)
                    {
                        case InterpolationType.Linear:
                            if (prev.time != after.time)
                            {
                                float fracx = (time - prev.time) / (after.time - prev.time);
                                return ((1 - fracx) * prev.value) + (fracx * after.value);
                            }
                            else
                                return prev.value;
                        case InterpolationType.Cubic:
                            if (prev.time != after.time)
                            {
                                float t = (time - prev.time) / (after.time - prev.time);
                                float a = 2 * (prev.value - after.value) + (1 * prev.tangent_out) + after.tangent_in;
                                float b = 3 * (after.value - prev.value) - (2 * prev.tangent_out) - after.tangent_in;
                                return ((a * t + b) * t + prev.tangent_out) * t + prev.value;
                            }
                            else
                                return prev.value;
                        default:
                            throw new Exception("Bck: Unknown interpolation type");
                    }
                }

                public Vector InterpolatePosition(float time, InterpolationType type = InterpolationType.Linear)
                {
                    InterpolateKeyFrameList(x, 257.5f, type);
                    Vector interpolation = new Vector(InterpolateKeyFrameList(x, time, type),
                                                      InterpolateKeyFrameList(y, time, type),
                                                      InterpolateKeyFrameList(z, time, type));
                    return interpolation;
                }
                public Vector InterpolateRotation(float time, InterpolationType type = InterpolationType.Linear)
                {
                    Vector interpolation = new Vector(InterpolateKeyFrameList(rotationx, time, type),
                                                      InterpolateKeyFrameList(rotationy, time, type),
                                                      InterpolateKeyFrameList(rotationz, time, type));
                    return interpolation;
                }
                public Vector InterpolateScale(float time, InterpolationType type = InterpolationType.Linear)
                {
                    Vector interpolation = new Vector(InterpolateKeyFrameList(scalex, time, type),
                                                      InterpolateKeyFrameList(scaley, time, type),
                                                      InterpolateKeyFrameList(scalez, time, type));
                    return interpolation;
                }
            }

            public Header hdr;
            public List<JointAnimation> jointanims = new List<JointAnimation>();
            public List<float> scales = new List<float>();
            public List<Int16> rotations = new List<Int16>();
            public List<float> translations = new List<float>();
            public float anglescale;

            public BckANK1(UInt32 size, Stream stream)
            {
                long secptr = stream.Position - 8;//section pointer

                this.magic = "ANK1";
                this.size = size;

                hdr = Header.Read(stream);//get header

                stream.Seek(secptr + hdr.jointanimationoffset, SeekOrigin.Begin);//go to the animations 
                for (int i = 0; i < hdr.jointanimationcount; i++)
                    jointanims.Add(JointAnimation.Read(stream));//read animations

                stream.Seek(secptr + hdr.scaleoffset, SeekOrigin.Begin);//go to the scales
                for (int i = 0; i < hdr.scalecount; i++)
                    scales.Add(Data.ReadSingle(stream));//read scales

                stream.Seek(secptr + hdr.rotoffset, SeekOrigin.Begin);//go to rotations
                for (int i = 0; i < hdr.rotcount; i++)
                    rotations.Add(Data.ReadInt16(stream));//read rotations

                stream.Seek(secptr + hdr.translationoffset, SeekOrigin.Begin);//go to translations
                for (int i = 0; i < hdr.translationcount; i++)
                    translations.Add(Data.ReadSingle(stream));//read translations
                    

                anglescale = (float)Math.Pow(2f, hdr.anglescaleexp) * (180f / 32767f);//some kind of float shenanigans

                stream.Seek(secptr + this.size, SeekOrigin.Begin);//seek the next section
            }

            private AnimKeyFrame[] GetScaleArray(int start, int count, int type)
            {
                int size = (type == 0 ? 3 : 4);
                AnimKeyFrame[] dat = new AnimKeyFrame[count];

                int ptr = start;
                int c = 0;
                while (c < count)
                {
                    dat[c] = new AnimKeyFrame();
                    if (count > 1)
                    {
                        dat[c].time = scales[ptr++];
                        dat[c].value = scales[ptr++];
                        dat[c].tangent_in = scales[ptr++];
                        dat[c].tangent_out = (type == 0 ? scales[ptr - 1] : scales[ptr++]);
                    }
                    else
                        dat[c].value = scales[ptr++];
                    c++;
                }
                return dat;
            }
            private AnimKeyFrame[] GetRotationArray(int start, int count, int type)
            {
                int size = (type == 0 ? 3 : 4);
                AnimKeyFrame[] dat = new AnimKeyFrame[count];

                int ptr = start;
                int c = 0;
                while (c < count)
                {
                    dat[c] = new AnimKeyFrame();
                    if (count > 1)
                    {
                        dat[c].time = rotations[ptr++];
                        dat[c].value = rotations[ptr++] * anglescale;
                        dat[c].tangent_in = rotations[ptr++];
                        dat[c].tangent_out = (type == 0 ? rotations[ptr - 1] : rotations[ptr++]);
                    }
                    else
                        dat[c].value = rotations[ptr++];
                    c++;
                }
                return dat;
            }
            private AnimKeyFrame[] GetTranslationArray(int start, int count, int type)
            {
                AnimKeyFrame[] dat = new AnimKeyFrame[count];//array of keyframes

                int ptr = start;
                int c = 0;
                while (c < count)
                {
                    dat[c] = new AnimKeyFrame();
                    if (count > 1)
                    {
                        dat[c].time = translations[ptr++];
                        dat[c].value = translations[ptr++];
                        dat[c].tangent_in = translations[ptr++];
                        dat[c].tangent_out = (type == 0 ? translations[ptr - 1] : translations[ptr++]);
                    }
                    else
                        dat[c].value = translations[ptr++];
                    c++;
                }
                return dat;
            }

            public Animation GetJointAnimation(int index)//
            {
                AnimKeyFrame[] x = GetTranslationArray(jointanims[index].x.translation.first, jointanims[index].x.translation.count, jointanims[index].x.translation.u1);
                AnimKeyFrame[] y = GetTranslationArray(jointanims[index].y.translation.first, jointanims[index].y.translation.count, jointanims[index].y.translation.u1);
                AnimKeyFrame[] z = GetTranslationArray(jointanims[index].z.translation.first, jointanims[index].z.translation.count, jointanims[index].z.translation.u1);

                AnimKeyFrame[] rx = GetRotationArray(jointanims[index].x.rotation.first, jointanims[index].x.rotation.count, jointanims[index].x.rotation.u1);
                AnimKeyFrame[] ry = GetRotationArray(jointanims[index].y.rotation.first, jointanims[index].y.rotation.count, jointanims[index].y.rotation.u1);
                AnimKeyFrame[] rz = GetRotationArray(jointanims[index].z.rotation.first, jointanims[index].z.rotation.count, jointanims[index].z.rotation.u1);

                AnimKeyFrame[] scalex = GetScaleArray(jointanims[index].x.scale.first, jointanims[index].x.scale.count, jointanims[index].x.scale.u1);
                AnimKeyFrame[] scaley = GetScaleArray(jointanims[index].y.scale.first, jointanims[index].y.scale.count, jointanims[index].y.scale.u1);
                AnimKeyFrame[] scalez = GetScaleArray(jointanims[index].z.scale.first, jointanims[index].z.scale.count, jointanims[index].z.scale.u1);

                Animation a = new Animation();
                a.x = new List<AnimKeyFrame>(x);
                a.y = new List<AnimKeyFrame>(y);
                a.z = new List<AnimKeyFrame>(z);
                a.rotationx = new List<AnimKeyFrame>(rx);
                a.rotationy = new List<AnimKeyFrame>(ry);
                a.rotationz = new List<AnimKeyFrame>(rz);
                a.scalex = new List<AnimKeyFrame>(scalex);
                a.scaley = new List<AnimKeyFrame>(scaley);
                a.scalez = new List<AnimKeyFrame>(scalez);
                return a;
            }
        }
    }
}
