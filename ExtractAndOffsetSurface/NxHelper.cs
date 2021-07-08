using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NXOpen;
using NXOpen.Assemblies;
using NXOpen.Facet;
using NXOpen.Features;
using NXOpen.GeometricUtilities;
using NXOpen.UF;
using NXOpen.Utilities;

namespace ExtractAndOffsetSurface
{
   
    public class NxHelper
    {
        private static Session _theSession;
        private static UI _theUi;
        private static UFSession _theUfSession;
        private static Part _theWorPart;
        public NxHelper(Session iNxSession)
        {
            _theSession = iNxSession;
            _theUfSession = UFSession.GetUFSession();
            _theUi = UI.GetUI();
            _theWorPart = _theSession.Parts.Work;
        }

        public ExtractFace ExtractFaces(Face[] iFaces)
        {
            BasePart iFaceOwningPart = null;
            Component iFaceOwningComponent = null;
            if (iFaces.Any())
            {
                Face iFace = iFaces[0];
                iFaceOwningPart = iFace.OwningPart;
                iFaceOwningComponent = iFace.OwningComponent;
            }
            if (iFaceOwningPart == null)
            {
                return null;
            }

            PartLoadStatus loadStatus;
            _theSession.Parts.SetWorkComponent(iFaceOwningComponent, out loadStatus);

            ExtractFaceBuilder extractFaceBuilder1 = _theSession.Parts.Work.Features.CreateExtractFaceBuilder(null);
            extractFaceBuilder1.ParentPart = ExtractFaceBuilder.ParentPartType.WorkPart;
            extractFaceBuilder1.DeleteHoles = false;
            extractFaceBuilder1.Associative = false;
            extractFaceBuilder1.FixAtCurrentTimestamp = false;
            extractFaceBuilder1.HideOriginal = true;
            extractFaceBuilder1.InheritDisplayProperties = false;
            extractFaceBuilder1.Type = ExtractFaceBuilder.ExtractType.Face;
            extractFaceBuilder1.ParentPart = ExtractFaceBuilder.ParentPartType.WorkPart;
            extractFaceBuilder1.FeatureOption = ExtractFaceBuilder.FeatureOptionType.OneFeatureForAllBodies;
            extractFaceBuilder1.FaceOption = ExtractFaceBuilder.FaceOptionType.FaceChain;

            Face[] boundaryFaces1 = new Face[0];
            FaceTangentRule faceTangentRule1;
            faceTangentRule1 = _theSession.Parts.Work.ScRuleFactory.CreateRuleFaceTangent(iFaces[0], boundaryFaces1, 0.5);

            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
            rules1[0] = faceTangentRule1;
            extractFaceBuilder1.FaceChain.ReplaceRules(rules1, false);


            NXObject nXObject1 = extractFaceBuilder1.Commit();
            extractFaceBuilder1.Destroy();

            if (nXObject1 != null)
            {
                ExtractFace extractedFace = nXObject1 as ExtractFace;
                if (extractedFace != null)
                {
                    Face[] faces = extractedFace.GetFaces();
                    NameAllFacesWithIncrementingNumbers(faces, "sc_edges");
                    Edge[] edges = extractedFace.GetEdges();
                    NameAllEdgesWithIncrementingNumbers(edges, "sc_edges");
                    return extractedFace;
                }
            }

            return null;

        }
        public List<FacetedBody> CreateFacetedBody(List<Body> iBodies)
        {
            FacetedBody[] facetBodies;
            List<FacetedBody> facetedBodiesNx = new List<FacetedBody>();
            int[] errorTable;
            _theSession.Parts.Work.FacetedBodies.CreateFacetBody(iBodies.ToArray(), out facetBodies, out errorTable);
            if (facetBodies.Length > 0)
            {
                foreach (FacetedBody facetedBodyJt in facetBodies)
                {
                    FacetedBody facetedBodyNx = _theSession.Parts.Work.FacetedBodies.Copy(facetedBodyJt, _theSession.Parts.Work,
                        FacetedBodyCollection.Type.Nx);
                    facetedBodiesNx.Add(facetedBodyNx);
                }
            }
            return facetedBodiesNx;
        }
        public void NameAllFacesWithIncrementingNumbers(Face[] iFaces,string faceName)
        {
            for (int i = 0; i < iFaces.Length; i++)
            {
                iFaces[i].SetName(faceName + "_" + i+1);
            }
        }
        public void NameAllEdgesWithIncrementingNumbers(Edge[] iEdges, string edgeName)
        {
            for (int i = 0; i < iEdges.Length; i++)
            {
                iEdges[i].SetName(edgeName + "_" + i + 1);
            }
        }
        public void DivideFaceFromCurves(List<Face> theFaces,List<Curve> theCurves )
        {
            DividefaceBuilder dividefaceBuilder1;
            dividefaceBuilder1 = _theSession.Parts.Work.Features.CreateDividefaceBuilder(null);

            dividefaceBuilder1.BlankOption = true;

            ProjectionOptions projectionOptions1;
            projectionOptions1 = dividefaceBuilder1.ProjectionOption;

            Section section1;
            section1 = dividefaceBuilder1.SelectDividingObject.CurvesToOffset;

            section1.DistanceTolerance = 0.0004;

            section1.ChainingTolerance = 0.00038;

            ScCollector scCollector1;
            scCollector1 = _theSession.Parts.Work.ScCollectors.CreateCollector();
            Face[] faces1 = new Face[theFaces.Count];
            for (int i = 0; i < theFaces.Count; i++)
            {
                faces1[i] = theFaces[i];
            }
            FaceDumbRule faceDumbRule1;
            faceDumbRule1 = _theSession.Parts.Work.ScRuleFactory.CreateRuleFaceDumb(faces1);
            SelectionIntentRule[] rules1 = new SelectionIntentRule[1];
            rules1[0] = faceDumbRule1;
            scCollector1.ReplaceRules(rules1, false);
            dividefaceBuilder1.FacesToDivide = scCollector1;

            Section section2;
            section2 = _theSession.Parts.Work.Sections.CreateSection(0.00038, 0.0004, 0.5);

            section2.SetAllowedEntityTypes(Section.AllowTypes.OnlyCurves);

            dividefaceBuilder1.SelectDividingObject.DividingObjectsList.Add(section2);

            IBaseCurve[] curves1 = new IBaseCurve[theCurves.Count];
            for (int i = 0; i < theCurves.Count; i++)
            {
                curves1[i] = theCurves[i];
            }
            CurveDumbRule curveDumbRule1;
            curveDumbRule1 = _theSession.Parts.Work.ScRuleFactory.CreateRuleBaseCurveDumb(curves1);

            section2.AllowSelfIntersection(true);

            SelectionIntentRule[] rules2 = new SelectionIntentRule[1];
            rules2[0] = curveDumbRule1;
            NXObject nullNXObject = null;
            Point3d helpPoint1 = new Point3d(0, 0, 0);
            section2.AddToSection(rules2, theCurves[0], nullNXObject, nullNXObject,
                helpPoint1, Section.Mode.Create, false);

            projectionOptions1.ProjectDirectionMethod =
                ProjectionOptions.DirectionType.FaceNormal;
            bool validate = dividefaceBuilder1.Validate();
            if (validate)
            {
                Feature feature1;
                feature1 = dividefaceBuilder1.CommitFeature();
            }
            dividefaceBuilder1.Destroy();
            
        }


        public void CreateTemporaryLine(double[] iStPnt, double[] iEndPnt)
        {
            View workView = _theWorPart.Views.WorkView;
            UFObj.DispProps dispProp = new UFObj.DispProps();
            dispProp.color = 16;
            _theUfSession.Disp.DisplayTemporaryLine(workView.Tag, UFDisp.ViewType.UseWorkView, iStPnt, iEndPnt,
                ref dispProp);
        }
        public TaggedObject[] GetObjectsOfSection(Section theSection)
        {
            ArrayList theObjects = new ArrayList();

            ScCollector scCollector1 = _theSession.Parts.Work.ScCollectors.CreateCollector();

            SectionData[] theSectionData;
            theSection.GetSectionData(out theSectionData);
            foreach (SectionData aSectionData in theSectionData)
            {
                SelectionIntentRule[] theRules;
                aSectionData.GetRules(out theRules);
                scCollector1.ReplaceRules(theRules, false);
                theObjects.AddRange(scCollector1.GetObjects());
            }

            return (TaggedObject[])theObjects.ToArray(typeof(TaggedObject));
        }
        public List<Curve> GetCurves(TaggedObject[] iTaggedObjects)
        {
            List<Curve> curves = new List<Curve>(0);
            List<Tag> curvetags = new List<Tag>(0);
            foreach (TaggedObject taggedObject in iTaggedObjects)
            {
                if (taggedObject is Edge)
                {
                    Edge edge = taggedObject as Edge;
                    Tag curveTag;
                    _theUfSession.Modl.CreateCurveFromEdge(edge.Tag, out curveTag);
                    if (curveTag != Tag.Null)
                    {
                        Curve curve = NXObjectManager.Get(curveTag) as Curve;
                        if (curve != null)
                        {
                            curves.Add(curve);
                            curvetags.Add(curve.Tag);
                        }
                    }
                }
            }

            Tag[] joinList = new Tag[curvetags.Count];
            int numJoin;
            //_theUfSession.Curve.AutoJoinCurves(curvetags.ToArray(), curvetags.Count, 1, joinList, out numJoin);
            return curves;
        }
        public Curve SimplifyCurves(List<Curve> iCurves)
        {
            return null;
        }

        static List<Curve>[] SortCurvesByConnectivity(Curve[] theCurves)
        {
            var markId1 = _theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "");
            List<Tag> theCurveTags = new List<Tag>();
            foreach (Curve aCurve in theCurves)
            {
                theCurveTags.Add(aCurve.Tag);
            }

            // AutoJoinCurves lets NX sort the curves into loops and/or chains
            int nJoined;
            Tag[] joined = new Tag[theCurveTags.Count];
            _theUfSession.Curve.AutoJoinCurves(theCurveTags.ToArray(), theCurveTags.Count,
                2, joined, out nJoined);

            List<Curve>[] curveLists =new List<Curve>[nJoined];
            for (int ii = 0; ii < nJoined; ii++)
                curveLists[ii] = new List<Curve>();

            foreach (Curve aCurve in theCurves)
            {
                double minDist;
                double[] junk = new double[3];
                double accuracy;
                int which = 0;
                _theUfSession.Modl.AskMinimumDist3(2, joined[0], aCurve.Tag, 0, junk, 0, junk,
                    out minDist, junk, junk, out accuracy);

                for (int ii = 1; ii < nJoined; ii++)
                {
                    double thisDist;
                    _theUfSession.Modl.AskMinimumDist3(2, joined[ii], aCurve.Tag, 0, junk, 0, junk,
                        out thisDist, junk, junk, out accuracy);
                    if (thisDist < minDist)
                    {
                        minDist = thisDist;
                        which = ii;
                    }
                }
                curveLists[which].Add(aCurve);
            }
            _theSession.UndoToMark(markId1, "");
            _theSession.DeleteUndoMark(markId1, "");
            return curveLists;
        }

        static List<List<Curve>> SortCurvesByConnectivity(List<Curve> theCurves)
        {
            var markId1 = _theSession.SetUndoMark(NXOpen.Session.MarkVisibility.Invisible, "");
            List<Tag> theCurveTags = new List<Tag>();
            foreach (Curve aCurve in theCurves)
            {
                theCurveTags.Add(aCurve.Tag);
            }

            // AutoJoinCurves lets NX sort the curves into loops and/or chains
            int nJoined;
            Tag[] joined = new Tag[theCurveTags.Count];
            _theUfSession.Curve.AutoJoinCurves(theCurveTags.ToArray(), theCurveTags.Count,
                2, joined, out nJoined);

            //List<Curve>[] curveLists = new List<Curve>[nJoined];
            List<List<Curve>> curveLists = new List<List<Curve>>();
            for (int ii = 0; ii < nJoined; ii++)
            {
                curveLists.Insert(ii,new List<Curve>());
            }

            foreach (Curve aCurve in theCurves)
            {
                double minDist;
                double[] junk = new double[3];
                double accuracy;
                int which = 0;
                _theUfSession.Modl.AskMinimumDist3(2, joined[0], aCurve.Tag, 0, junk, 0, junk,
                    out minDist, junk, junk, out accuracy);

                for (int ii = 1; ii < nJoined; ii++)
                {
                    double thisDist;
                    _theUfSession.Modl.AskMinimumDist3(2, joined[ii], aCurve.Tag, 0, junk, 0, junk,
                        out thisDist, junk, junk, out accuracy);
                    if (thisDist < minDist)
                    {
                        minDist = thisDist;
                        which = ii;
                    }
                }
                curveLists[which].Add(aCurve);
            }
            _theSession.UndoToMark(markId1, "");
            _theSession.DeleteUndoMark(markId1, "");
            return curveLists;
        }

        public StudioSpline CreateStudioSplineFromPointList(List<Point3d> pl)
        {
            NXObject nullNXObject = null;
            NXOpen.Features.StudioSplineBuilderEx studioSplineBuilderEx1;
            studioSplineBuilderEx1 = _theSession.Parts.Work.Features.CreateStudioSplineBuilderEx(nullNXObject);
            studioSplineBuilderEx1.MatchKnotsType = NXOpen.Features.StudioSplineBuilderEx.MatchKnotsTypes.None;
            studioSplineBuilderEx1.OrientExpress.ReferenceOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Reference.WcsDisplayPart;
            studioSplineBuilderEx1.OrientExpress.AxisOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Axis.Passive;
            studioSplineBuilderEx1.OrientExpress.PlaneOption = NXOpen.GeometricUtilities.OrientXpressBuilder.Plane.Passive;
            studioSplineBuilderEx1.DrawingPlaneOption = NXOpen.Features.StudioSplineBuilderEx.DrawingPlaneOptions.XY;
            studioSplineBuilderEx1.Degree = 1;

            foreach (Point3d p in pl)
            {
                NXOpen.Features.GeometricConstraintData geometricConstraintData1;
                geometricConstraintData1 = studioSplineBuilderEx1.ConstraintManager.CreateGeometricConstraintData();
                geometricConstraintData1.Point = _theSession.Parts.Work.Points.CreatePoint(p);
                studioSplineBuilderEx1.ConstraintManager.Append(geometricConstraintData1);
            }

            NXObject nXObject1;
            nXObject1 = studioSplineBuilderEx1.Commit();
            studioSplineBuilderEx1.Destroy();

            return (StudioSpline)nXObject1;
        }
    }
}
