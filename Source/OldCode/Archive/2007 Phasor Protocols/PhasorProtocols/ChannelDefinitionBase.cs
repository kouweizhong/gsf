//*******************************************************************************************************
//  ChannelDefinitionBase.cs
//  Copyright © 2009 - TVA, all rights reserved - Gbtc
//
//  Build Environment: C#, Visual Studio 2008
//  Primary Developer: James R Carroll
//      Office: PSO TRAN & REL, CHATTANOOGA - MR BK-C
//       Phone: 423/751-4165
//       Email: jrcarrol@tva.gov
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  03/07/2005 - James R Carroll
//       Generated original version of source code.
//
//*******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace PCS.PhasorProtocols
{
    /// <summary>
    /// Represents the common implementation of the protocol independent definition of any kind of <see cref="IChannel"/> data.
    /// </summary>
    [Serializable()]
    public abstract class ChannelDefinitionBase : ChannelBase, IChannelDefinition
    {
        #region [ Members ]

        // Fields
        private IConfigurationCell m_parent;
        private int m_index;
        private string m_label;
        private uint m_scale;
        private double m_offset;

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Creates a new <see cref="ChannelDefinitionBase"/>.
        /// </summary>
        protected ChannelDefinitionBase()
        {
        }

        /// <summary>
        /// Creates a new <see cref="ChannelDefinitionBase"/> from serialization parameters.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> with populated with data.</param>
        /// <param name="context">The source <see cref="StreamingContext"/> for this deserialization.</param>
        protected ChannelDefinitionBase(SerializationInfo info, StreamingContext context)
        {
            // Deserialize channel definition
            m_parent = (IConfigurationCell)info.GetValue("parent", typeof(IConfigurationCell));
            m_index = info.GetInt32("index");
            this.Label = info.GetString("label");
            m_scale = info.GetUInt32("scale");
            m_offset = info.GetDouble("offset");
        }

        /// <summary>
        /// Creates a new <see cref="ChannelDefinitionBase"/> from specified parameters.
        /// </summary>
        protected ChannelDefinitionBase(IConfigurationCell parent, int index, string label, uint scale, double offset)
        {
            m_parent = parent;
            m_index = index;
            this.Label = label;
            m_scale = scale;
            m_offset = offset;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the <see cref="IConfigurationCell"/> parent of this <see cref="ChannelDefinitionBase"/>.
        /// </summary>
        public virtual IConfigurationCell Parent
        {
            get
            {
                return m_parent;
            }
        }

        /// <summary>
        /// Gets the <see cref="PhasorProtocols.DataFormat"/> of this <see cref="ChannelDefinitionBase"/>.
        /// </summary>
        public abstract DataFormat DataFormat { get; }

        /// <summary>
        /// Gets or sets the index of this <see cref="ChannelDefinitionBase"/>.
        /// </summary>
        public virtual int Index
        {
            get
            {
                return m_index;
            }
            set
            {
                m_index = value;
            }
        }

        /// <summary>
        /// Gets or sets the offset of this <see cref="ChannelDefinitionBase"/>.
        /// </summary>
        public virtual double Offset
        {
            get
            {
                return m_offset;
            }
            set
            {
                m_offset = value;
            }
        }

        /// <summary>
        /// Gets or sets the conversion factor of this <see cref="ChannelDefinitionBase"/>.
        /// </summary>
        public virtual double ConversionFactor
        {
            get
            {
                return m_scale * ScalePerBit;
            }
            set
            {
                unchecked
                {
                    ScalingFactor = (uint)(value / ScalePerBit);
                }
            }
        }

        /// <summary>
        /// Gets the scale/bit value of this <see cref="ChannelDefinitionBase"/>.
        /// </summary>
        /// <remarks>
        /// The base implementation assumes scale/bit of 10^-5.
        /// </remarks>
        public virtual double ScalePerBit
        {
            get
            {
                // Typical scale/bit is 10^-5
                return 0.00001D;
            }
        }

        /// <summary>
        /// Gets or sets the integer based scaling factor of this <see cref="ChannelDefinitionBase"/>.
        /// </summary>
        public virtual uint ScalingFactor
        {
            get
            {
                return m_scale;
            }
            set
            {
                if (value > MaximumScalingFactor)
                    throw new OverflowException("Scaling factor value cannot exceed " + MaximumScalingFactor);

                m_scale = value;
            }
        }

        /// <summary>
        /// Gets the maximum value for the <see cref="ScalingFactor"/> of this <see cref="ChannelDefinitionBase"/>.
        /// </summary>
        /// <remarks>
        /// The base implementation accomodates maximum scaling factor of 24-bits of space.
        /// </remarks>
        public virtual uint MaximumScalingFactor
        {
            get
            {
                // Typical scaling/conversion factors should fit within 3 bytes (i.e., 24 bits) of space
                return UInt24.MaxValue;
            }
        }

        /// <summary>
        /// Gets or sets the label of this <see cref="ChannelDefinitionBase"/>.
        /// </summary>
        public virtual string Label
        {
            get
            {
                return m_label;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    value = "undefined";

                value = value.GetValidLabel();

                if (value.Length > MaximumLabelLength)
                    throw new OverflowException("Label length cannot exceed " + MaximumLabelLength + " characters.");

                m_label = value;
            }
        }

        /// <summary>
        /// Gets the binary image of the <see cref="Label"/> of this <see cref="ChannelDefinitionBase"/>.
        /// </summary>
        public virtual byte[] LabelImage
        {
            get
            {
                return Encoding.ASCII.GetBytes(m_label.PadRight(MaximumLabelLength));
            }
        }

        /// <summary>
        /// Gets the maximum length of the <see cref="Label"/> of this <see cref="ChannelDefinitionBase"/>.
        /// </summary>
        public virtual int MaximumLabelLength
        {
            get
            {
                // Typical label length is 16 characters
                return 16;
            }
        }

        /// <summary>
        /// Gets the length of the <see cref="BodyImage"/>.
        /// </summary>
        protected override int BodyLength
        {
            get
            {
                return MaximumLabelLength;
            }
        }

        /// <summary>
        /// Gets the binary body image of the <see cref="ChannelDefinitionBase"/> object.
        /// </summary>
        protected override byte[] BodyImage
        {
            get
            {
                return LabelImage;
            }
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> of string based property names and values for this <see cref="ChannelDefinitionBase"/> object.
        /// </summary>
        public override Dictionary<string, string> Attributes
        {
            get
            {
                Dictionary<string, string> baseAttributes = base.Attributes;

                baseAttributes.Add("Label", Label);
                baseAttributes.Add("Index", Index.ToString());
                baseAttributes.Add("Offset", Offset.ToString());
                baseAttributes.Add("Data Format", (int)DataFormat + ": " + DataFormat);
                baseAttributes.Add("Scaling Factor", ScalingFactor.ToString());
                baseAttributes.Add("Scale per Bit", ScalePerBit.ToString());
                baseAttributes.Add("Maximum Scaling Factor", MaximumScalingFactor.ToString());
                baseAttributes.Add("Conversion Factor", ConversionFactor.ToString());
                baseAttributes.Add("Maximum Label Length", MaximumLabelLength.ToString());

                return baseAttributes;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Parses the binary body image.
        /// </summary>
        /// <param name="binaryImage">Binary image to parse.</param>
        /// <param name="startIndex">Start index into <paramref name="binaryImage"/> to begin parsing.</param>
        /// <param name="length">Length of valid data within <paramref name="binaryImage"/>.</param>
        /// <returns>The length of the data that was parsed.</returns>
        /// <remarks>
        /// The base implementation assumes that all channel defintions begin with a label as this is
        /// the general case, override functionality if this is not the case.
        /// </remarks>
        protected override int ParseBodyImage(byte[] binaryImage, int startIndex, int length)
        {
            // TODO: It is expected that parent IConfigurationCell will validate that it has
            // enough length to parse entire cell well in advance so that low level parsing
            // routines do not have to re-validate that enough length is available to parse
            // needed information as an optimization...

            Label = Encoding.ASCII.GetString(binaryImage, startIndex, MaximumLabelLength);

            return MaximumLabelLength;
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare, or null.</param>
        /// <returns>
        /// True if obj is an instance of <see cref="IChannelDefinition"/> and equals the value of this instance;
        /// otherwise, False.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as IChannelDefinition);
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to specified <see cref="IChannelDefintion"/> value.
        /// </summary>
        /// <param name="obj">A <see cref="IChannelDefinition"/> object to compare to this instance.</param>
        /// <returns>
        /// True if obj has the same value as this instance; otherwise, False.
        /// </returns>
        public bool Equals(IChannelDefinition obj)
        {
            return (CompareTo(obj) == 0);
        }

        /// <summary>
        /// Compares this instance to a specified object and returns an indication of their relative values.
        /// </summary>
        /// <param name="obj">An object to compare, or null.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value. Returns less than zero
        /// if this instance is less than value, zero if this instance is equal to value, or greater than zero
        /// if this instance is greater than value.
        /// </returns>
        /// <exception cref="ArgumentException">value is not an UInt32 or UInt24.</exception>
        public virtual int CompareTo(object obj)
        {
            IChannelDefinition other = obj as IChannelDefinition;
            
            if (other != null)
                return CompareTo(other);

            throw new ArgumentException("ChannelDefinition can only be compared to other IChannelDefinitions");
        }

        /// <summary>
        /// Compares this instance to a specified <see cref="IChannelDefintion"/> object and returns an indication of their
        /// relative values.
        /// </summary>
        /// <param name="value">A <see cref="IChannelDefintion"/> object to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and value. Returns less than zero
        /// if this instance is less than value, zero if this instance is equal to value, or greater than zero
        /// if this instance is greater than value.
        /// </returns>
        public int CompareTo(IChannelDefinition other)
        {
            // We sort channel Definitions by index
            return m_index.CompareTo(other.Index);
        }

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination <see cref="StreamingContext"/> for this serialization.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize channel definition
            info.AddValue("parent", m_parent, typeof(IConfigurationCell));
            info.AddValue("index", m_index);
            info.AddValue("label", m_label);
            info.AddValue("scale", m_scale);
            info.AddValue("offset", m_offset);
        }

        #endregion
    }
}