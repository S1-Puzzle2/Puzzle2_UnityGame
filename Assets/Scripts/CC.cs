﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class CC : MonoBehaviour
{

    private WebCamTexture wb;
    GameObject obj;
    Color32[] diffPic = null;
    int[] luma = null;
    int index = 0;
    int pixelWidth;
    int pixelHeight;
    int segmentsX = 40;//64*2;
    int segmentsY = 40;//48*2;
    int[,] segments;
    int XpixelInSegment;
    int YpixelInSegment;
    int max1X, max2X;
    int max1Y, max2Y;
    int max1Val, max2Val;
    int middle, yMiddle;
    
    Vector2 player1;
    Vector2 player2;
    
    int minRow, maxRow, minColumn, maxColumn;
    public Color32 borderColor = new Color32(150, 0, 0, 0);
    bool first = true;
    public bool playDifference = true;
    public GameObject sphere1;
    public GameObject sphere2;
    public int MinimumLuma = 3;
    int  minLuma = 10;
    int minimalLumaMatrix = 10;
    bool initialized = false;
    bool called = false;
    float XFactor; 
    float YFactor;
    public GameObject gameField;
    Vector3 _gameField;
    public bool searchForGamefield = false;

    float posXp1, posYp1, posXp2, posYp2;


    // Use this for initialization
    void Start()
    {

    }

    void Awake()
    {

        wb = new WebCamTexture("Microsoft LifeCam HD-3000");
        wb.Play();
        

    }

    private Color32[] getColorFromLuma(int[] lumas)
    {
        Color32[] temp = new Color32[lumas.Length];
        byte l = 0;
        Color32 black = new Color32(0, 0, 0, 0);

        for (int i = 0; i < lumas.Length; ++i)
        {
            l = (byte)lumas[i];
            if (l > 0)
            {

                temp[i] = new Color32(l, l, l, 0);
            }
            else
            {
                temp[i] = black;
            }
        }
        return temp;
    }

    // Update is called once per frame
    void Update()
    {

        if (initialized)
        {
            if (first)
            {
                init();
                first = false;
            }

            Color32[] difference = CalculateDifference(wb.GetPixels32());

            int max = 0;
            int index = 0;
            int[] luma = getLuma(difference);

            calcMaxPos2();

            mapSphere();

            if (difference != null && playDifference)
            {

                Texture2D texture = new Texture2D(wb.width, wb.height, TextureFormat.ARGB32, false);
                texture.SetPixels32(difference);
                texture.Apply();

                renderer.material.mainTexture = texture;
            }
        }
        else
        {
            if(!called)
                StartCoroutine(initGameField());
            
            //initGameField();
            
        }
    }

    private IEnumerator initGameField()
    {
        called = true;
        yield return new WaitForSeconds(0.5f);
        init(); 

        if (searchForGamefield)
        {
            
            Color32[] pic = wb.GetPixels32();
            int line, yPos, xPos;
            double dLine;
            LinkedList<int> borderX = new LinkedList<int>();
            LinkedList<int> borderY = new LinkedList<int>();

            for (int i = 0; i < pic.Length; i++)
            {
                if (pic[i].r > borderColor.r)
                {
                    line = i / pixelWidth;
                    dLine = (double)i / pixelWidth;
                    yPos = line / YpixelInSegment;
                    xPos = (int)((dLine - line) * pixelWidth / XpixelInSegment);

                    if (!borderX.Contains(xPos))
                        borderX.AddLast(xPos);

                    if (!borderY.Contains(yPos))
                        borderY.AddLast(yPos);

                }
            }

            //int minRow, maxRow, minColumn, maxColumn;
            bool first = true;
            int temp;
            for (int i = 0; i < borderX.Count; i++)
            {
                if (first)
                {
                    minColumn = borderX.ElementAt(i);
                    maxColumn = minColumn;
                    first = false;
                }
                else
                {
                    temp = borderX.ElementAt(i);
                    if (temp < minColumn)
                    {
                        minColumn = temp;
                    }
                    else if (temp > maxColumn)
                    {
                        maxColumn = temp;
                    }
                }
            }
            first = true;
            for (int i = 0; i < borderY.Count; i++)
            {
                if (first)
                {
                    minRow = borderY.ElementAt(i);
                    maxRow = minRow;
                    first = false;
                }
                else
                {
                    temp = borderY.ElementAt(i);
                    if (temp < minRow)
                    {
                        minRow = temp;
                    }
                    else if (temp > maxRow)
                    {
                        maxRow = temp;
                    }
                }
            }
        }
        else
        {
            minColumn = 0;
            maxColumn = segmentsX;
            minRow = 0;
            maxRow = segmentsY;
        }
        middle = minColumn + (maxColumn - minColumn) / 2;
        yMiddle = minRow + (maxRow - minRow) / 2;
        posXp1 =  _gameField.x / (maxColumn - minColumn);
        posYp1 =  _gameField.y / (maxRow - minRow);
        posXp2 = -((_gameField.x) / 2);
        posYp2 = -((_gameField.y) / 2);
        //Debug.Log("minRow: " + minRow +"\nmaxRow: " + maxRow + "\nminCol: " + minColumn + "\nmaxCol: " + maxColumn);
        initialized = true;
    
    }

    private void mapSphere()
    {

        
        float sphere1X, sphere1Y,sphere2X, sphere2Y;
        


        sphere1X = posXp2 + player1.x * posXp1;
        sphere1Y = posYp2 + player1.y * posYp1;
        sphere2X = posXp2 + player2.x * posXp1;
        sphere2Y = posYp2 + player2.y * posYp1;

       // Debug.Log(player1.x + "/" + player1.y + "   |||||   " + sphere1X + "/" + sphere1Y);
        
        /*
        sphere1X = (player1.x - minRow) * (Screen.width) / ((maxRow - minRow));
        sphere1Y = (player1.y -minColumn) * (Screen.height) / (maxColumn - minColumn);

        sphere2X = (player2.x - minRow) * (Screen.width) / (maxRow - minRow);
        sphere2Y = (player2.y - minColumn) * (Screen.height) / (maxColumn - minColumn);
        */
        Vector3 pos1 = new Vector3(sphere1X, sphere1Y, 0.0f);
        //Vector3 worldPos1 = Camera.main.ScreenToWorldPoint(pos1);
        Vector3 pos2 = new Vector3(sphere2X, sphere2Y, 0.0f);
        //Vector3 worldPos2 = Camera.main.ScreenToWorldPoint(pos2);
       
        //worldPos1.z = 0.0f;
        //worldPos2.z = 0.0f;

        sphere1.transform.position = Vector3.Lerp(sphere1.transform.position, pos1, Time.deltaTime*2);
        sphere2.transform.position = Vector3.Lerp(sphere2.transform.position, pos2, Time.deltaTime*2);
        

    }

    private void init()
    {
        pixelWidth = wb.width;
        pixelHeight = wb.height;
        XpixelInSegment = pixelWidth / segmentsX;
        YpixelInSegment = pixelHeight / segmentsY;
        minimalLumaMatrix = (XpixelInSegment * YpixelInSegment) * MinimumLuma;
        segments = new int[segmentsX, segmentsY];
        _gameField = gameField.renderer.bounds.size;
       
    }

    private void calcMaxPos2()
    {
        //Debug.Log("minCol: " + minColumn + " / middle: " + middle + " / max: " + maxColumn);
        //Debug.Log("minRow: " + minRow + " / middle:" + yMiddle + " / max: " + maxRow);
       player1 = calcMaxPos(minColumn, middle, 1);
       player2 = calcMaxPos(middle, maxColumn, 2);
    }


    private Vector2 calcMaxPos(int minimalColumn,int maximalColumn, int player)
    {
        int _maxX=0, _maxY=0, _maxVal =0;

        for (int x = minimalColumn; x < maximalColumn; ++x)
        {
            for (int y = minRow; y < maxRow; ++y)
            {
                if (segments[x, y] > _maxVal )
                {
                    _maxX = x;
                    _maxY = y;
                    _maxVal = segments[x, y];
                }
            }
        }
        if (_maxVal > minimalLumaMatrix)
        {
            if (player == 1)
            {
                max1X = _maxX;
                max1Y = _maxY;
                max1Val = _maxVal;  
            }
            else
            {
                max2X = _maxX;
                max2Y = _maxY;
                max2Val = _maxVal;
            }
            
        }

        if (player == 1)
        {
           // return new Vector3(31, 0);
           return new Vector2(max1X-minColumn, max1Y-minRow);
        }
        else 
        {
            return new Vector2(max2X-minColumn, max2Y-minRow);
        }

        return new Vector2(0,0);

    }

    private int[] getLuma(Color32[] color32)
    {
        int[] temp = new int[color32.Length];
        clearSegments();
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = (color32[i].r + color32[i].r + color32[i].r + color32[i].b + color32[i].g + color32[i].g + color32[i].g + color32[i].g) >> 3;

            int hans = temp[i];

            if (temp[i] > minLuma)
                addLumaInMatrix(i, temp[i]);
        }

        return temp;
    }

    private void clearSegments()
    {
        for (int x = 0; x < segmentsX; ++x)
        {
            for (int y = 0; y < segmentsY; ++y)
            {
                segments[x, y] = 0;
            }
        }
    }

    private void addLumaInMatrix(int i, int lumaVal)
    {
        int line = i / pixelWidth;
        double dLine = (double)i / pixelWidth;
        int yPos = line / YpixelInSegment;
        int xPos = (int)((dLine - line) * pixelWidth / XpixelInSegment);

        if (  posValid(xPos,yPos))
            segments[xPos, yPos] += lumaVal;

    }

    private bool posValid(int xPos, int yPos)
    {
        return (xPos < segmentsX) && (yPos < segmentsY) && (xPos >= minRow && xPos <= maxRow) && (yPos >= minColumn && yPos <= maxColumn);
    }

    private Color32[] CalculateDifference(Color32[] color32)
    {
        Color32[] pic = null;

        if (diffPic == null)
        {
            diffPic = color32;
            return color32;
        }
        else
        {
            pic = (Color32[])color32.Clone();
            int diff = 1;
            Color32 black = new Color32(0, 0, 0, 0);

            for (int i = 0; i < pic.Length; ++i)
            {

                pic[i].r = (Mathf.Abs(pic[i].r - diffPic[i].r) < diff ? (byte)0 : (byte)Mathf.Abs(pic[i].r - diffPic[i].r));
                pic[i].g = (Mathf.Abs(pic[i].g - diffPic[i].g) < diff ? (byte)0 : (byte)Mathf.Abs(pic[i].g - diffPic[i].g));
                pic[i].b = (Mathf.Abs(pic[i].b - diffPic[i].b) < diff ? (byte)0 : (byte)Mathf.Abs(pic[i].b - diffPic[i].b));
            }

            diffPic = (Color32[])color32.Clone(); 
        }
        return pic;
    }
}
