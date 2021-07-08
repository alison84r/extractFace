﻿//==============================================================================
//  WARNING!!  This file is overwritten by the Block UI Styler while generating
//  the automation code. Any modifications to this file will be lost after
//  generating the code again.
//
//       Filename:  D:\Personal\Projects\MBRDI\ExtractAndOffsetSurface\ExtractAndOffsetUI.cs
//
//        This file was generated by the NX Block UI Styler
//        Created by: snagareddy
//              Version: NX 12
//              Date: 07-06-2021  (Format: mm-dd-yyyy)
//              Time: 22:52 (Format: hh-mm)
//
//==============================================================================

//==============================================================================
//  Purpose:  This TEMPLATE file contains C# source to guide you in the
//  construction of your Block application dialog. The generation of your
//  dialog file (.dlx extension) is the first step towards dialog construction
//  within NX.  You must now create a NX Open application that
//  utilizes this file (.dlx).
//
//  The information in this file provides you with the following:
//
//  1.  Help on how to load and display your Block UI Styler dialog in NX
//      using APIs provided in NXOpen.BlockStyler namespace
//  2.  The empty callback methods (stubs) associated with your dialog items
//      have also been placed in this file. These empty methods have been
//      created simply to start you along with your coding requirements.
//      The method name, argument list and possible return values have already
//      been provided for you.
//==============================================================================

//------------------------------------------------------------------------------
//These imports are needed for the following template code
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using ExtractAndOffsetSurface;
using NXOpen;
using NXOpen.BlockStyler;
using NXOpen.Features;
using NXOpen.UIStyler;
using Group = NXOpen.BlockStyler.Group;
using Separator = NXOpen.BlockStyler.Separator;

//------------------------------------------------------------------------------
//Represents Block Styler application class
//------------------------------------------------------------------------------
public class ExtractAndOffsetUI
{
    //class members
    private static Session theSession;
    private static UI theUI;
    private string theDlxFileName;
    private NxHelper nxHelper;
    private BlockDialog theDialog;
    private Group group0;// Block type: Group
    private FaceCollector SelectPaintFaceBtn;// Block type: Face Collector
    private SpecifyPoint point0;// Block type: Specify Point
    private Separator separator0;// Block type: Separator
    private FaceCollector OverlapFacesBtn;// Block type: Face Collector
    private SectionBuilder SelectExistingEdgesBtn;// Block type: Section Builder
    private SectionBuilder FreeHandCurveBtn;// Block type: Section Builder
    private Separator separator01;// Block type: Separator
    private Button PaintBtn;// Block type: Button

    #region Painting Module variables

    //Painting Module variables
    private ExtractFace _paintingFace;
    

    #endregion


    //------------------------------------------------------------------------------
    //Constructor for NX Styler class
    //------------------------------------------------------------------------------
    public ExtractAndOffsetUI()
    {
        try
        {
            theSession = Session.GetSession();
            theUI = UI.GetUI();
            theDlxFileName =
                @"D:\Personal\Projects\MBRDI\ExtractAndOffsetSurface\ExtractAndOffsetSurface\ExtractAndOffsetSurface\ExtractAndOffsetUI.dlx";
            theDialog = theUI.CreateDialog(theDlxFileName);
            theDialog.AddUpdateHandler(update_cb);
            theDialog.AddInitializeHandler(initialize_cb);
            theDialog.AddDialogShownHandler(dialogShown_cb);
            nxHelper = new NxHelper(theSession);
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            throw ex;
        }
    }

    //------------------------------- DIALOG LAUNCHING ---------------------------------
    //
    //    Before invoking this application one needs to open any part/empty part in NX
    //    because of the behavior of the blocks.
    //
    //    Make sure the dlx file is in one of the following locations:
    //        1.) From where NX session is launched
    //        2.) $UGII_USER_DIR/application
    //        3.) For released applications, using UGII_CUSTOM_DIRECTORY_FILE is highly
    //            recommended. This variable is set to a full directory path to a file 
    //            containing a list of root directories for all custom applications.
    //            e.g., UGII_CUSTOM_DIRECTORY_FILE=$UGII_BASE_DIR\ugii\menus\custom_dirs.dat
    //
    //    You can create the dialog using one of the following way:
    //
    //    1. Journal Replay
    //
    //        1) Replay this file through Tool->Journal->Play Menu.
    //
    //    2. USER EXIT
    //
    //        1) Create the Shared Library -- Refer "Block UI Styler programmer's guide"
    //        2) Invoke the Shared Library through File->Execute->NX Open menu.
    //
    //------------------------------------------------------------------------------
    public static void Main()
    {
        ExtractAndOffsetUI theExtractAndOffsetUI = null;
        try
        {
            theExtractAndOffsetUI = new ExtractAndOffsetUI();
            // The following method shows the dialog immediately
            theExtractAndOffsetUI.Show();
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        finally
        {
            if (theExtractAndOffsetUI != null)
                theExtractAndOffsetUI.Dispose();
            theExtractAndOffsetUI = null;
        }
    }
    //------------------------------------------------------------------------------
    // This method specifies how a shared image is unloaded from memory
    // within NX. This method gives you the capability to unload an
    // internal NX Open application or user  exit from NX. Specify any
    // one of the three constants as a return value to determine the type
    // of unload to perform:
    //
    //
    //    Immediately : unload the library as soon as the automation program has completed
    //    Explicitly  : unload the library from the "Unload Shared Image" dialog
    //    AtTermination : unload the library when the NX session terminates
    //
    //
    // NOTE:  A program which associates NX Open applications with the menubar
    // MUST NOT use this option since it will UNLOAD your NX Open application image
    // from the menubar.
    //------------------------------------------------------------------------------
    public static int GetUnloadOption(string arg)
    {
        //return System.Convert.ToInt32(Session.LibraryUnloadOption.Explicitly);
        return Convert.ToInt32(Session.LibraryUnloadOption.Immediately);
        // return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
    }

    //------------------------------------------------------------------------------
    // Following method cleanup any housekeeping chores that may be needed.
    // This method is automatically called by NX.
    //------------------------------------------------------------------------------
    public static void UnloadLibrary(string arg)
    {
        try
        {
            
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
    }

    //------------------------------------------------------------------------------
    //This method shows the dialog on the screen
    //------------------------------------------------------------------------------
    public DialogResponse Show()
    {
        try
        {
            theDialog.Show();
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return 0;
    }

    //------------------------------------------------------------------------------
    //Method Name: Dispose
    //------------------------------------------------------------------------------
    public void Dispose()
    {
        if (theDialog != null)
        {
            theDialog.Dispose();
            theDialog = null;
        }
    }

    //------------------------------------------------------------------------------
    //---------------------Block UI Styler Callback Functions--------------------------
    //------------------------------------------------------------------------------

    //------------------------------------------------------------------------------
    //Callback Name: initialize_cb
    //------------------------------------------------------------------------------
    public void initialize_cb()
    {
        try
        {
            group0 = (Group)theDialog.TopBlock.FindBlock("group0");
            SelectPaintFaceBtn = (FaceCollector)theDialog.TopBlock.FindBlock("SelectPaintFaceBtn");
            point0 = (SpecifyPoint)theDialog.TopBlock.FindBlock("point0");
            separator0 = (Separator)theDialog.TopBlock.FindBlock("separator0");
            OverlapFacesBtn = (FaceCollector)theDialog.TopBlock.FindBlock("OverlapFacesBtn");
            SelectExistingEdgesBtn = (SectionBuilder)theDialog.TopBlock.FindBlock("SelectExistingEdgesBtn");
            FreeHandCurveBtn = (SectionBuilder)theDialog.TopBlock.FindBlock("FreeHandCurveBtn");
            separator01 = (Separator)theDialog.TopBlock.FindBlock("separator01");
            PaintBtn = (Button)theDialog.TopBlock.FindBlock("PaintBtn");
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
    }

    //------------------------------------------------------------------------------
    //Callback Name: dialogShown_cb
    //This callback is executed just before the dialog launch. Thus any value set 
    //here will take precedence and dialog will be launched showing that value. 
    //------------------------------------------------------------------------------
    public void dialogShown_cb()
    {
        try
        {
            PaintBtn.Enable = false;
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
    }

    //------------------------------------------------------------------------------
    //Callback Name: update_cb
    //------------------------------------------------------------------------------
    public int update_cb(UIBlock block)
    {
        try
        {
            if (block == SelectPaintFaceBtn)
            {
                TaggedObject[] selectedObjects = SelectPaintFaceBtn.GetSelectedObjects();
                if (selectedObjects.Any())
                {
                    PaintBtn.Enable = true;
                }
                else
                {
                    PaintBtn.Enable = false;
                }
            }
            else if (block == point0)
            {
                //---------Enter your code here-----------
            }
            else if (block == separator0)
            {
                //---------Enter your code here-----------
            }
            else if (block == OverlapFacesBtn)
            {
                //---------Enter your code here-----------
            }
            else if (block == SelectExistingEdgesBtn)
            {
                //---------Enter your code here-----------
            }
            else if (block == FreeHandCurveBtn)
            {
                //---------Enter your code here-----------
                TaggedObject[] selectedObjects = FreeHandCurveBtn.GetSelectedObjects();
                List<Point3d> curvepoint3d = new List<Point3d>(0);
                foreach (TaggedObject selectedObject in selectedObjects)
                {
                    if (selectedObject is Section)
                    {
                        TaggedObject[] objectsOfSection = nxHelper.GetObjectsOfSection(selectedObject as Section);
                        foreach (TaggedObject taggedObject in objectsOfSection)
                        {
                            if (taggedObject is Point)
                            {
                                Point point = taggedObject as Point;
                                curvepoint3d.Add(point.Coordinates);
                            }
                        }

                    }
                }

                if (curvepoint3d.Count >= 1)
                {
                    StudioSpline studioSpline = nxHelper.CreateStudioSplineFromPointList(curvepoint3d);
                    if (studioSpline != null)
                    {
                        studioSpline.SetName("SP_SURFACE_COAT_DELETE");
                    }
                }
            }
            else if (block == separator01)
            {
                //---------Enter your code here-----------
            }
            else if (block == PaintBtn)
            {
                #region 1. Extract selected paint faces

                TaggedObject[] selectedObjects = SelectPaintFaceBtn.GetSelectedObjects();
                List<Face> selectedFaces = new List<Face>(0);
                foreach (TaggedObject selectedObject in selectedObjects)
                {
                    if (selectedObject is Face)
                    {
                        // UFFacet ufFacet = theUfSession.Facet;
                        selectedFaces.Add(selectedObject as Face);
                    }
                }

                if (selectedFaces.Any())
                {
                    _paintingFace = nxHelper.ExtractFaces(selectedFaces.ToArray());
                    nxHelper.CreateFacetedBody(_paintingFace.GetBodies().ToList());
                }

                #endregion

                #region Get Sections curves and Create a non associative curves

                TaggedObject[] selectedSecObjects = SelectExistingEdgesBtn.GetSelectedObjects();
                List<Edge> edges3d = new List<Edge>(0);
                List<Curve> curves = new List<Curve>(0);
                foreach (TaggedObject selectedObject in selectedSecObjects)
                {
                    if (selectedObject is Section)
                    {
                        TaggedObject[] objectsOfSection = nxHelper.GetObjectsOfSection(selectedObject as Section);
                        curves = nxHelper.GetCurves(objectsOfSection);
                    }
                }

                if (curves.Any())
                {
                    //Divide directly on the surface
                    nxHelper.DivideFaceFromCurves(_paintingFace.GetFaces().ToList(), curves);
                }
                #endregion


                //Reset the dialogue
                theDialog.PerformApply();

            }
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return 0;
    }

    //------------------------------------------------------------------------------
    //Function Name: GetBlockProperties
    //Returns the propertylist of the specified BlockID
    //------------------------------------------------------------------------------
    public PropertyList GetBlockProperties(string blockID)
    {
        PropertyList plist = null;
        try
        {
            plist = theDialog.GetBlockProperties(blockID);
        }
        catch (Exception ex)
        {
            //---- Enter your exception handling code here -----
            theUI.NXMessageBox.Show("Block Styler", NXMessageBox.DialogType.Error, ex.ToString());
        }
        return plist;
    }

}
