﻿//*******************************************************************************************************
//  WaveFormatChunk.cs
//  Copyright © 2008 - TVA, all rights reserved - Gbtc
//
//  Build Environment: C#, Visual Studio 2008
//  Primary Developer: James R Carroll
//      Office: PSO TRAN & REL, CHATTANOOGA - MR BK-C
//       Phone: 423/751-4165
//       Email: jrcarrol@tva.gov
//
//  Code Modification History:
//  -----------------------------------------------------------------------------------------------------
//  10/03/2008 - James R Carroll
//       Generated original version of source code.
//
//*******************************************************************************************************

using System;
using System.IO;

namespace TVA.Media
{
    /// <summary>
    /// Represents the format chunk in a WAVE media format file.
    /// </summary>
    public class WaveFormatChunk : RiffChunk
    {
        private short m_audioFormat;
        private short m_channels;
        private int m_sampleRate;
        private int m_byteRate;
        private short m_blockAlignment;
        private short m_bitsPerSample;
        private short m_extraParametersSize;
        private byte[] m_extraParameters;

        public const string RiffTypeID = "fmt ";

        public WaveFormatChunk(int sampleRate, short bitsPerSample, short channels, short audioFormat)
            : base(RiffTypeID)
        {
            if (bitsPerSample % 8 != 0)
                throw new InvalidDataException("Invalid bit rate specified - wave file bit rates must be a multiple of 8");

            m_sampleRate = sampleRate;
            m_bitsPerSample = bitsPerSample;
            m_channels = channels;
            m_audioFormat = audioFormat;

            UpdateByteRate();
            UpdateBlockAlignment();
        }

        /// <summary>Reads a new WAVE format section from the specified stream.</summary>
        /// <param name="preRead">Pre-parsed RIFF chunk header.</param>
        /// <param name="source">Source stream to read data from.</param>
        /// <exception cref="InvalidOperationException">WAVE format or extra parameters section too small, wave file corrupted.</exception>
        public WaveFormatChunk(RiffChunk preRead, Stream source)
            : base(preRead, RiffTypeID)
        {
            int length = BinaryLength - preRead.BinaryLength;
            byte[] buffer = new byte[length];

            int bytesRead = source.Read(buffer, 0, length);

            if (bytesRead < length)
                throw new InvalidOperationException("WAVE format section too small, wave file corrupted.");

            Initialize(buffer, 0);

            // Read extra parameters, if any
            if (m_extraParametersSize > 0)
            {
                m_extraParameters = new byte[m_extraParametersSize];

                bytesRead = source.Read(m_extraParameters, 0, m_extraParametersSize);

                if (bytesRead < m_extraParametersSize)
                    throw new InvalidOperationException("WAVE extra parameters section too small, wave file corrupted.");
            }
        }

        public override int Initialize(byte[] binaryImage, int startIndex)
        {
            m_audioFormat = BitConverter.ToInt16(binaryImage, startIndex);
            m_channels = BitConverter.ToInt16(binaryImage, startIndex + 2);
            m_sampleRate = BitConverter.ToInt32(binaryImage, startIndex + 4);
            m_byteRate = BitConverter.ToInt32(binaryImage, startIndex + 8);
            m_blockAlignment = BitConverter.ToInt16(binaryImage, startIndex + 12);
            m_bitsPerSample = BitConverter.ToInt16(binaryImage, startIndex + 14);

            if (m_bitsPerSample % 8 != 0)
                throw new InvalidDataException("Invalid bit rate encountered - wave file bit rates must be a multiple of 8");

            m_extraParametersSize = BitConverter.ToInt16(binaryImage, startIndex + 16);

            return BinaryLength;
        }

        public override byte[] BinaryImage
        {
            get
            {
                byte[] binaryImage = new byte[BinaryLength];
                int startIndex = base.BinaryLength;

                Buffer.BlockCopy(base.BinaryImage, 0, binaryImage, 0, startIndex);
                Buffer.BlockCopy(BitConverter.GetBytes(m_audioFormat), 0, binaryImage, startIndex, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(m_channels), 0, binaryImage, startIndex + 2, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(m_sampleRate), 0, binaryImage, startIndex + 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(m_byteRate), 0, binaryImage, startIndex + 8, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(m_blockAlignment), 0, binaryImage, startIndex + 12, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(m_bitsPerSample), 0, binaryImage, startIndex + 14, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(m_extraParametersSize), 0, binaryImage, startIndex + 16, 2);

                if (m_extraParametersSize > 0 && m_extraParameters != null)
                    Buffer.BlockCopy(m_extraParameters, 0, binaryImage, startIndex + 18, m_extraParametersSize);

                return binaryImage;
            }
        }

        public override int BinaryLength
        {
            get
            {
                return base.BinaryLength + 18 + m_extraParametersSize;
            }
        }

        public short AudioFormat
        {
            get
            {
                return m_audioFormat;
            }
            set
            {
                m_audioFormat = value;
            }
        }

        public short Channels
        {
            get
            {
                return m_channels;
            }
            set
            {
                m_channels = value;
                UpdateByteRate();
                UpdateBlockAlignment();
            }
        }

        public int SampleRate
        {
            get
            {
                return m_sampleRate;
            }
            set
            {
                m_sampleRate = value;
                UpdateByteRate();
            }
        }

        public short BlockAlignent
        {
            get
            {
                return m_blockAlignment;
            }
            set
            {
                m_blockAlignment = value;
            }
        }

        public short BitsPerSample
        {
            get
            {
                return m_bitsPerSample;
            }
            set
            {
                m_bitsPerSample = value;
                UpdateByteRate();
                UpdateBlockAlignment();
            }
        }

        public short ExtraParametersSize
        {
            get
            {
                return m_extraParametersSize;
            }
        }

        public byte[] ExtraParameters
        {
            get
            {
                return m_extraParameters;
            }
            set
            {
                m_extraParameters = value;

                if (m_extraParameters == null)
                    m_extraParametersSize = 0;
                else
                    m_extraParametersSize = (short)m_extraParameters.Length;
            }
        }

        private void UpdateByteRate()
        {
            m_byteRate = m_sampleRate * m_channels * (m_bitsPerSample / 8);
        }

        private void UpdateBlockAlignment()
        {
            m_blockAlignment = (short)(m_channels * (m_bitsPerSample / 8));
        }
    }
}
