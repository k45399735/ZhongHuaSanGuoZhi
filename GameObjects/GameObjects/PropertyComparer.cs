﻿namespace GameObjects
{
    using GameGlobal;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class PropertyComparer : IComparer<GameObject>
    {
        private bool isNumber;
        private string propertyName;
        private bool SmallToBig;
        private int itemID;

        public PropertyComparer(string propertyName, bool isNumber, bool SmallToBig)
        {
            this.propertyName = propertyName;
            this.isNumber = isNumber;
            this.SmallToBig = SmallToBig;
            this.itemID = -1;
        }

        public PropertyComparer(string propertyName, bool isNumber, bool SmallToBig, int itemID)
        {
            this.propertyName = propertyName;
            this.isNumber = isNumber;
            this.SmallToBig = SmallToBig;
            this.itemID = itemID;
        }

        private static Regex dateMatcher = new Regex("^(\\d+)年(1?\\d)月([123]?\\d)日$", RegexOptions.Compiled);
        private static Regex slashMatcher = new Regex("^(\\d+)/(\\d+)$", RegexOptions.Compiled);
        private static Regex slashStringMatcher = new Regex("^(\\d+)/(.*)$", RegexOptions.Compiled);
        public int Compare(GameObject x, GameObject y)
        {
            if ((x == null) || (y == null))
            {
                return 0;
            }

            if (x.Equals(y))
            {
                return 0;
            }
            int result = 0;

            Object objX, objY;

            if (itemID < 0)
            {
                objX = StaticMethods.GetPropertyValue(x, this.propertyName);
                objY = StaticMethods.GetPropertyValue(y, this.propertyName);
            }
            else
            {
                objX = StaticMethods.GetMethodValue(x, this.propertyName, new object[] { this.itemID });
                objY = StaticMethods.GetMethodValue(y, this.propertyName, new object[] { this.itemID });
            }

            if (this.isNumber)
            {
                try
                {
                    int longResult = (int)objX - (int)objY;
                    if (longResult > 0)
                    {
                        result = 1;
                    }
                    else if (longResult < 0)
                    {
                        result = -1;
                    }
                    else
                    {
                        result = 0;
                    }
                }
                catch (InvalidCastException)
                {
                    try
                    {
                        long longResult = (long)objX - (long)objY;
                        if (longResult > 0)
                        {
                            result = 1;
                        }
                        else if (longResult < 0)
                        {
                            result = -1;
                        }
                        else
                        {
                            result = 0;
                        }
                    }
                    catch (InvalidCastException)
                    {
                        try
                        {
                            if (Math.Abs((double)objX - (double)objY) < 0.00001) return 0;
                            result = (((double)objX) > ((double)objY)) ? 1 : -1;
                        }
                        catch (InvalidCastException)
                        {
                            result = -1;
                        }
                    }
                }
            }
            else
            {
                String xStr = objX.ToString();
                String yStr = objY.ToString();
                Match xMatch = slashMatcher.Match(xStr);
                Match yMatch = slashMatcher.Match(yStr);

                if (xMatch.Success && yMatch.Success)
                {
                    int xLeft = int.Parse(xMatch.Groups[1].ToString());
                    int xRight = int.Parse(xMatch.Groups[2].ToString());
                    int yLeft = int.Parse(yMatch.Groups[1].ToString());
                    int yRight = int.Parse(yMatch.Groups[2].ToString());
                    result = xRight == yRight ? xLeft - yLeft : xRight - yRight;
                }
                else
                {
                    xMatch = slashStringMatcher.Match(xStr);
                    yMatch = slashStringMatcher.Match(yStr);
                    if (xMatch.Success && yMatch.Success)
                    {
                        int xv = int.Parse(xMatch.Groups[1].ToString());
                        int yv = int.Parse(yMatch.Groups[1].ToString());
                        result = xv - yv;
                    }
                    else
                    {
                        xMatch = dateMatcher.Match(xStr);
                        yMatch = dateMatcher.Match(yStr);
                        if (xMatch.Success && yMatch.Success)
                        {
                            int xYear = int.Parse(xMatch.Groups[1].ToString());
                            int xMonth = int.Parse(xMatch.Groups[2].ToString());
                            int xDay = int.Parse(xMatch.Groups[3].ToString());
                            int yYear = int.Parse(yMatch.Groups[1].ToString());
                            int yMonth = int.Parse(yMatch.Groups[2].ToString());
                            int yDay = int.Parse(yMatch.Groups[3].ToString());
                            if (xYear == yYear)
                            {
                                if (xMonth == yMonth)
                                {
                                    result = xDay - yDay;
                                }
                                else
                                {
                                    result = xMonth - yMonth;
                                }
                            }
                            else
                            {
                                result = xYear - yYear;
                            }
                        }
                        else
                        {
                            result = xStr.CompareTo(yStr);
                        }
                    }
                }
            }

            if (!this.SmallToBig)
            {
                result = -result;
            }

            return result;
        }
    }
}

