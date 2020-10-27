using System;
using System.IO;

namespace Planner.WpfViewModels.Notes.Pasters
{
	//A bitmap file is a device independant bitmap with a 14 byte headder prepended to it.
	//when you get a DIB from the clipboard, the 14 byte file headder is  missing, but can
	// be calculated from the stuff that you can find.  This stream does some bit mangling on
	// the first read operation to add the 14 byte headder to the front of the stream
	public class BitmapPrefixStream : Stream
	{
		private bool firstBlockDone = false;
		private Stream innerStream;
		const int fileHeaderLength = 14;
		const int bitmapHeaderLength = 40;

		public BitmapPrefixStream(Stream innerStream)
		{
			this.innerStream = innerStream;
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (firstBlockDone) return innerStream.Read(buffer, offset, count);
			firstBlockDone = true;
			if (count - offset < fileHeaderLength + bitmapHeaderLength)
				throw new InvalidOperationException("Buffer To Small");
			if (offset != 0) throw new InvalidOperationException("Buffer Misaligned");
			var bytesRead = innerStream.Read(buffer, fileHeaderLength, count - fileHeaderLength);

			var reader = new BitReader(buffer, fileHeaderLength);

			int infoHeaderSize = reader.BiSize();
			int fileSize = fileHeaderLength + infoHeaderSize + reader.BiSizeImage();

			var bw = new BitWriter(buffer);
			bw.WriteInt(0x4d42, 0, 2);
			bw.WriteInt(fileSize, 2, 4);
			bw.WriteInt(fileHeaderLength + infoHeaderSize + reader.BiClrUsed() * 4, 10, 4);
			return fileHeaderLength + bytesRead;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new System.NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new System.NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new System.NotSupportedException();
		}

		public override bool CanRead => true;
		public override bool CanSeek => false;
		public override bool CanWrite => false;
		public override long Length => fileHeaderLength + innerStream.Length;
		public override long Position { get; set; }
	}

	struct BitWriter
	{
		private byte[] bytes;

		public BitWriter(byte[] target)
		{
			bytes = target;
		}

		public void WriteInt(int value, int position, int len)
		{
			for (int i = 0; i < len; i++)
			{
				bytes[position + i] = (byte) (value & 0xff);
				value >>= 8;
			}
		}
	}


	struct BitReader
	{
		private byte[] target;
		private int offset;

		public BitReader(byte[] target, int offset)
		{
			this.target = target;
			this.offset = offset;
		}

		private int ReadInt(int outerOffset)
		{
			int position = outerOffset + offset;
			int ret = 0;
			for (int i = 3; i >= 0; i--)
			{
				ret <<= 8;
				ret |= target[position + i];
			}

			return ret;
		}

		public int BiSize() => ReadInt(0);
		public int BiSizeImage() => ReadInt(20);
		public int BiClrUsed() => ReadInt(32);
	}
}