namespace SillyWidgets
{
    public interface ISillySegmentVisitor
    {
        void VisitController(SillyControllerSegment controller);
        void VisitMethod(SillyMethodSegment method);
        void VisitVariable(SillyVariableSegment variable);
        void VisitStatic(SillyHardCodedSegment hardCoded);
    }
}