using System;
using System.Collections.Generic;


namespace SillyWidgets
{
    public enum SegmentTypes { Controller, Method, Variable, HardCoded }

    public abstract class SillySegment
    {
        public string Value { get; private set; }
        public SegmentTypes Type { get; protected set; }

        public SillySegment(string text, SegmentTypes type)
        {
            Value = text;
            Type = type;
        }

        public abstract void Visit(ISillySegmentVisitor visitor);
    }

    public class SillyHardCodedSegment : SillySegment
    {
        public bool IsRoot { get; private set; }

        public SillyHardCodedSegment(string text)
            : base(text, SegmentTypes.HardCoded)
        {
            IsRoot = false;
        }

        public SillyHardCodedSegment()
            : this("/")
        {
            IsRoot = true;
        }

        public override void Visit(ISillySegmentVisitor visitor)
        {
            visitor.VisitStatic(this);
        }
    }

    public abstract class SillyReservedSegment : SillySegment
    {
        public SillyReservedSegment(string defaultValue, SegmentTypes type)
            : base(defaultValue, type)
        {

        }
    }

    public class SillyControllerSegment :SillyReservedSegment
    {
        public SillyControllerSegment(string defaultController)
            : base (defaultController, SegmentTypes.Controller)
        {

        }

        public override void Visit(ISillySegmentVisitor visitor)
        {
            visitor.VisitController(this);
        }
    }

    public class SillyMethodSegment : SillyReservedSegment
    {
        public SillyMethodSegment(string defaultMethod)
            : base(defaultMethod, SegmentTypes.Method)
        {

        }

        public override void Visit(ISillySegmentVisitor visitor)
        {
            visitor.VisitMethod(this);
        }
    }

    public class SillyVariableSegment : SillySegment
    {
        public string Default { get; private set; }
        
        public SillyVariableSegment(string name, string defaultValue)
            : base(name, SegmentTypes.Variable)
        {
        }

        public override void Visit(ISillySegmentVisitor visitor)
        {
            visitor.VisitVariable(this);
        }

        public bool DefaultExists()
        {
            return(String.IsNullOrEmpty(Default));
        }

        private string StripBraces(string name)
        {
            return(name.Trim(new char[] { '}', '{' }));
        }
    }
}