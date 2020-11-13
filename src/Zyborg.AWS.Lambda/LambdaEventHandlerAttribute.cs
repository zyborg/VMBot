using System;

namespace Zyborg.AWS.Lambda
{
    /// <summary>
    /// Use this attribute to mark methods on a <see cref="MultiEventFunction{TResult}"/>
    /// subclass as Handler for different events.
    /// </summary>
    /// <remarks>
    /// This attribute can be configured to mark a method to handle different
    /// typed Lambda Events, or events that can be tested for with a custom matcher
    /// method, or a single default handler.
    /// <p>A single default handler is specified by not providing either
    /// the <see cref="LambdaEventHandlerAttribute.MatcherMethod" /> or
    /// the <see cref="LambdaEventHandlerAttribute.EventType" /> parameter.
    /// Note only a single method can be marekd as the default handler, otherwise
    /// an exception is thrown during registration.
    /// </p><p>
    /// A typed Lambda Event handler is specified by providing a type for
    /// the <see cref="LambdaEventHandlerAttribute.EventType" /> parameter.
    /// However, this Event Type <i>must</i> be registered in advance with
    /// the underlying decoder, by using the
    /// <see cref="MultiEventFunction{TResult}.Register{TEvent}(LambdaJsonEventDecoder{TResult}.LambdaEventMatcher)"/>
    /// method, such as in the method
    /// <see cref="MultiEventFunction{TResult}.RegisterEventTypeMatchers"/>.
    /// </p><p>
    /// And finally, a handler can be registered using a custom Matcher by
    /// providing the name of an instance method to the 
    /// the <see cref="LambdaEventHandlerAttribute.MatcherMethod" /> parameter.
    /// </p>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class LambdaEventHandlerAttribute : Attribute
    {
        public LambdaEventHandlerAttribute()
        {
            // This is the "DEFAULT" handler -- you should only declare one of these
        }

        public LambdaEventHandlerAttribute(Type eventType)
        {
            EventType = eventType;
        }

        public LambdaEventHandlerAttribute(string matcherMethod)
        {
            MatcherMethod = matcherMethod;
        }

        public Type EventType { get; }

        public string MatcherMethod { get; }

        public bool IsDefaultHandler => EventType == null && MatcherMethod == null;
    }
}