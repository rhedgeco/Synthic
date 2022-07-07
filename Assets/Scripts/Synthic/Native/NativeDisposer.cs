using System;

public class NativeDisposer<T> where T: unmanaged, IDisposable
{
    public T Object;

    public static explicit operator T(NativeDisposer<T> disposer)
    {
        return disposer.Object;
    }

    // Destructor will make sure that the object is disposed when garbage collected
    ~NativeDisposer()
    {
        Object.Dispose();
    }
}