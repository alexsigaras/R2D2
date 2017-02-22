#if PocketPC || Smartphone || WindowsCE
#define CompactFW
#else
#define NotCompactFW
#endif

using System;
using System.Diagnostics;
using System.Threading;

namespace NKH.MindSqualls
{
    /// <summary>
    /// <para>This class is primarily used for internal debugging, tracing and profiling off the MindSqualls API. To use it you will have to change the class access modifier from "internal" to "public".</para>
    /// </summary>
    internal class TraceUtil
    {
        /// <summary>
        /// <para>Call this method when entering a method.</para>
        /// </summary>
        public static void MethodEnter()
        {
#if NotCompactFW
            StackTrace st = new StackTrace(true);
            int indentLevel = st.GetFrames().Length - 1;
            Trace.IndentLevel = indentLevel;
            if (indentLevel > 0) Trace.IndentLevel--;

            StackFrame sf = st.GetFrame(1);

            Trace.WriteLine(
                string.Format("{0} :: {1}",
                    sf.GetMethod().ReflectedType.FullName,
                    sf.GetMethod().Name),
                string.Format("Enter ({0}, {1})",
                    DateTime.Now.ToString("HH:mm:ss:fff"),
                    Thread.CurrentThread.ManagedThreadId
                    )
                );
#endif
        }

        /// <summary>
        /// <para>Call this method when exiting a method.</para>
        /// </summary>
        /// <param name="message">An optional message</param>
        public static void MethodExit(string message)
        {
#if NotCompactFW
            StackTrace st = new StackTrace(true);
            int indentLevel = st.GetFrames().Length - 1;
            Trace.IndentLevel = indentLevel;
            if (indentLevel > 0) Trace.IndentLevel--;

            StackFrame sf = st.GetFrame(1);

            if (message != null && message != "")
                Trace.WriteLine(message);

            Trace.WriteLine(
                string.Format("{0} :: {1}",
                    sf.GetMethod().ReflectedType.FullName,
                    sf.GetMethod().Name),
                string.Format("Exit  ({0}, {1})",
                    DateTime.Now.ToString("HH:mm:ss:fff"),
                    Thread.CurrentThread.ManagedThreadId
                    )
            );
#endif
        }

        /// <summary>
        /// <para>Call this method to indicate that a method has been visited.</para>
        /// <para>This class sorts of combines MethodEnter() and MethodExit().</para>
        /// </summary>
        /// <seealso cref="MethodEnter"/>
        /// <seealso cref="MethodExit"/>
        public static void MethodVisit()
        {
#if NotCompactFW
            StackTrace st = new StackTrace(true);
            int indentLevel = st.GetFrames().Length - 1;
            Trace.IndentLevel = indentLevel;
            if (indentLevel > 0) Trace.IndentLevel--;

            StackFrame sf = st.GetFrame(1);

            Trace.WriteLine(
                string.Format("{0} :: {1}",
                    sf.GetMethod().ReflectedType.FullName,
                    sf.GetMethod().Name),
                string.Format("Visit ({0}, {1})",
                    DateTime.Now.ToString("HH:mm:ss:fff"),
                    Thread.CurrentThread.ManagedThreadId
                    )
            );
#endif
        }

        /// <summary>
        /// <para>Use this method to enter a note with the trace.</para>
        /// </summary>
        /// <param name="note">The note</param>
        public static void Note(string note)
        {
#if NotCompactFW
            StackTrace st = new StackTrace(true);
            int indentLevel = st.GetFrames().Length - 1;
            Trace.IndentLevel = indentLevel;
            if (indentLevel > 0) Trace.IndentLevel--;

            StackFrame sf = st.GetFrame(1);

            Trace.WriteLine(
                note,
                string.Format("Note  ({0}, {1})",
                    DateTime.Now.ToString("HH:mm:ss:fff"),
                    Thread.CurrentThread.ManagedThreadId
                    )
            );
#endif
        }
    }
}
