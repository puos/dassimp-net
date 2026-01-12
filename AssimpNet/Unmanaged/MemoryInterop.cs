using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Assimp.Unmanaged
{
    public static class MemoryInterop
    {
        public static unsafe int SizeOfInline<T>() where T : struct
        {
            return Unsafe.SizeOf<T>();
        }

        public static unsafe void* AsPointerInline<T>(ref T src) where T : struct
        {
            fixed(T* pSrc = &src)
                return pSrc;
        }

        public static unsafe void* AsPointerReadonlyInline<T>(in T src) where T : struct
        {
            fixed(T* pSrc = &src)
                return pSrc;
        }

        public static unsafe ref T AsRef<T>(IntPtr pSrc) where T : struct
        {
            return ref Unsafe.AsRef<T>((void*)pSrc);
        }

        public static unsafe ref TTo As<TFrom, TTo>(ref TFrom src) where TFrom : struct where TTo : struct
        {
            return ref Unsafe.As<TFrom, TTo>(ref src);
        }

        public static unsafe ref readonly TTo AsReadonly<TFrom, TTo>(in TFrom src) where TFrom : struct where TTo : struct
        {
            return ref Unsafe.As<TFrom, TTo>(ref Unsafe.AsRef(in src));
        }

        public static unsafe void MemSetUnalignedInline(void* dest, byte value, uint size)
        {
            Span<byte> view = new Span<byte>(dest, (int)size);
            view.Fill(value);
        }

        public static unsafe void MemCopyUnalignedInline(void* dest, void* src, uint size)
        {
            Span<byte> srcView = new Span<byte>(src, (int)size);
            Span<byte> destView = new Span<byte>(dest, (int)size);
            srcView.CopyTo(destView);
        }

        public static unsafe void ReadArrayUnaligned<T>(IntPtr src, T[] dest, int startIndexInArray, int count) where T : struct
        {
            if(dest == null)
                throw new ArgumentNullException(nameof(dest));

            int size = Unsafe.SizeOf<T>();
            fixed (T* pDest = &dest[startIndexInArray])
            {
                Buffer.MemoryCopy((void*)src, pDest, size * count, size * count);
            }
        }

        public static unsafe void WriteArrayUnaligned<T>(IntPtr dest, T[] src, int startIndexInArray, int count) where T : struct
        {
            if(src == null)
                throw new ArgumentNullException(nameof(src));

            int size = Unsafe.SizeOf<T>();
            fixed (T* pSrc = &src[startIndexInArray])
            {
                Buffer.MemoryCopy(pSrc, (void*)dest, size * count, size * count);
            }
        }

        public static unsafe T ReadInline<T>(void* src) where T : struct
        {
            return Unsafe.ReadUnaligned<T>(src);
        }

        public static unsafe void WriteInline<T>(void* dest, in T data) where T : struct
        {
            Unsafe.WriteUnaligned(dest, data);
        }
    }
}