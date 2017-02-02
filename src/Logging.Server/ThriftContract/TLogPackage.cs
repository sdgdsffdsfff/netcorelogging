/**
 * Autogenerated by Thrift Compiler (0.9.2)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */

using System;
using System.Collections.Generic;
using System.Text;
using Thrift.Protocol;

namespace Logging.ThriftContract
{
#if !SILVERLIGHT

    [Serializable]
#endif
    public partial class TLogPackage : TBase
    {
        private long _IP;
        private int _AppId;
        private List<TLogEntity> _LogItems;
        private List<TMetricEntity> _MetricItems;

        public long IP
        {
            get
            {
                return _IP;
            }
            set
            {
                __isset.IP = true;
                this._IP = value;
            }
        }

        public int AppId
        {
            get
            {
                return _AppId;
            }
            set
            {
                __isset.AppId = true;
                this._AppId = value;
            }
        }

        public List<TLogEntity> LogItems
        {
            get
            {
                return _LogItems;
            }
            set
            {
                __isset.LogItems = true;
                this._LogItems = value;
            }
        }

        public List<TMetricEntity> MetricItems
        {
            get
            {
                return _MetricItems;
            }
            set
            {
                __isset.MetricItems = true;
                this._MetricItems = value;
            }
        }

        public Isset __isset;
#if !SILVERLIGHT

        [Serializable]
#endif
        public struct Isset
        {
            public bool IP;
            public bool AppId;
            public bool LogItems;
            public bool MetricItems;
        }

        public TLogPackage()
        {
        }

        public void Read(TProtocol iprot)
        {
            TField field;
            iprot.ReadStructBegin();
            while (true)
            {
                field = iprot.ReadFieldBegin();
                if (field.Type == TType.Stop)
                {
                    break;
                }
                switch (field.ID)
                {
                    case 1:
                        if (field.Type == TType.I64)
                        {
                            IP = iprot.ReadI64();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 2:
                        if (field.Type == TType.I32)
                        {
                            AppId = iprot.ReadI32();
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 3:
                        if (field.Type == TType.List)
                        {
                            {
                                LogItems = new List<TLogEntity>();
                                TList _list10 = iprot.ReadListBegin();
                                for (int _i11 = 0; _i11 < _list10.Count; ++_i11)
                                {
                                    TLogEntity _elem12;
                                    _elem12 = new TLogEntity();
                                    _elem12.Read(iprot);
                                    LogItems.Add(_elem12);
                                }
                                iprot.ReadListEnd();
                            }
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    case 4:
                        if (field.Type == TType.List)
                        {
                            {
                                MetricItems = new List<TMetricEntity>();
                                TList _list13 = iprot.ReadListBegin();
                                for (int _i14 = 0; _i14 < _list13.Count; ++_i14)
                                {
                                    TMetricEntity _elem15;
                                    _elem15 = new TMetricEntity();
                                    _elem15.Read(iprot);
                                    MetricItems.Add(_elem15);
                                }
                                iprot.ReadListEnd();
                            }
                        }
                        else
                        {
                            TProtocolUtil.Skip(iprot, field.Type);
                        }
                        break;

                    default:
                        TProtocolUtil.Skip(iprot, field.Type);
                        break;
                }
                iprot.ReadFieldEnd();
            }
            iprot.ReadStructEnd();
        }

        public void Write(TProtocol oprot)
        {
            TStruct struc = new TStruct("TLogPackage");
            oprot.WriteStructBegin(struc);
            TField field = new TField();
            if (__isset.IP)
            {
                field.Name = "IP";
                field.Type = TType.I64;
                field.ID = 1;
                oprot.WriteFieldBegin(field);
                oprot.WriteI64(IP);
                oprot.WriteFieldEnd();
            }
            if (__isset.AppId)
            {
                field.Name = "AppId";
                field.Type = TType.I32;
                field.ID = 2;
                oprot.WriteFieldBegin(field);
                oprot.WriteI32(AppId);
                oprot.WriteFieldEnd();
            }
            if (LogItems != null && __isset.LogItems)
            {
                field.Name = "LogItems";
                field.Type = TType.List;
                field.ID = 3;
                oprot.WriteFieldBegin(field);
                {
                    oprot.WriteListBegin(new TList(TType.Struct, LogItems.Count));
                    foreach (TLogEntity _iter16 in LogItems)
                    {
                        _iter16.Write(oprot);
                    }
                    oprot.WriteListEnd();
                }
                oprot.WriteFieldEnd();
            }
            if (MetricItems != null && __isset.MetricItems)
            {
                field.Name = "MetricItems";
                field.Type = TType.List;
                field.ID = 4;
                oprot.WriteFieldBegin(field);
                {
                    oprot.WriteListBegin(new TList(TType.Struct, MetricItems.Count));
                    foreach (TMetricEntity _iter17 in MetricItems)
                    {
                        _iter17.Write(oprot);
                    }
                    oprot.WriteListEnd();
                }
                oprot.WriteFieldEnd();
            }
            oprot.WriteFieldStop();
            oprot.WriteStructEnd();
        }

        public override string ToString()
        {
            StringBuilder __sb = new StringBuilder("TLogPackage(");
            bool __first = true;
            if (__isset.IP)
            {
                if (!__first) { __sb.Append(", "); }
                __first = false;
                __sb.Append("IP: ");
                __sb.Append(IP);
            }
            if (__isset.AppId)
            {
                if (!__first) { __sb.Append(", "); }
                __first = false;
                __sb.Append("AppId: ");
                __sb.Append(AppId);
            }
            if (LogItems != null && __isset.LogItems)
            {
                if (!__first) { __sb.Append(", "); }
                __first = false;
                __sb.Append("LogItems: ");
                __sb.Append(LogItems);
            }
            if (MetricItems != null && __isset.MetricItems)
            {
                if (!__first) { __sb.Append(", "); }
                __first = false;
                __sb.Append("MetricItems: ");
                __sb.Append(MetricItems);
            }
            __sb.Append(")");
            return __sb.ToString();
        }
    }
}