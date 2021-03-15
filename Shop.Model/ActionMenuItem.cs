﻿using System;

namespace Shop.Model
{
    public class ActionMenuItem : MenuItem
    {
        public Func<IResult> Action { get; private set; }

        public ActionMenuItem(string text, Func<IResult> action) : base(text)
        {
            Action = action;
        }
    }
}
