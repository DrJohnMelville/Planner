using System;
using System.IO;

namespace Planner.WpfViewModels.Notes.Pasters
{
	//A bitmap file is a device independant bitmap with a 14 byte header prepended to it.
	//when you get a DIB from the clipboard, the 14 byte file header is  missing, but can
	// be calculated from the stuff that you can find.  This stream does some bit mangling on
	// the first read operation to add the 14 byte header to the front of the stream
	public class BitmapPrefixStream : Stream
	{
		private bool firstBlockDone = false;
		private Stream innerStream;
		const int fileHeaderLength = 14;
		const int bitmapHeaderLength = 40;

		public BitmapPrefixStream(Stream innerStream) => this.innerStream = innerStream;
		
		public override int Read(byte[] buffer, int offset, int count) =>
			firstBlockDone ? innerStream.Read(buffer, offset, count) : ReadFirstBlock(buffer, offset, count);

		private int ReadFirstBlock(byte[] buffer, int offset, int count)
		{
			firstBlockDone = true;
			VerifyValidBufferForPrefix(offset, count);
			var bytesRead = innerStream.Read(buffer, fileHeaderLength, count - fileHeaderLength);
			FillHeaderBlock(buffer);
			return fileHeaderLength + bytesRead;
		}

		private static void VerifyValidBufferForPrefix(int offset, int count)
		{
			if (count - offset < fileHeaderLength + bitmapHeaderLength)
				throw new InvalidOperationException("Buffer To Small");
			if (offset != 0) throw new InvalidOperationException("Buffer Misaligned");
		}

		private static void FillHeaderBlock(byte[] buffer)
		{
			var reader = new BitReader(buffer, fileHeaderLength);
			var fuillHeadderLength = fileHeaderLength + reader.BiSize();
			int fileSize = fuillHeadderLength + reader.BiSizeImage();
			var bitmapDataStart = fuillHeadderLength + reader.BiClrUsed() * 4;
			WriteHeaderBlockValues(buffer, fileSize, bitmapDataStart);
		}

		private static void WriteHeaderBlockValues(byte[] buffer, int fileSize, int fullHeadderLength)
		{
			var bw = new BitWriter(buffer);
			bw.WriteInt(0x4d42, 0, 2);
			bw.WriteInt(fileSize, 2, 4);
			bw.WriteInt(fullHeadderLength, 10, 4);
		}

		#region Irrelevant overrides

		public override long Seek(long offset, SeekOrigin origin) => 
			throw new NotSupportedException();

		public override void SetLength(long value) => 
			throw new NotSupportedException();

		public override void Write(byte[] buffer, int offset, int count) => 
			throw new NotSupportedException();

		public override bool CanRead => true;
		public override bool CanSeek => false;
		public override bool CanWrite => false;
		public override long Length => fileHeaderLength + innerStream.Length;
		public override long Position { get; set; }
		public override void Flush()
		{
		}
		#endregion


		private struct BitWriter
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


		private struct BitReader
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
}