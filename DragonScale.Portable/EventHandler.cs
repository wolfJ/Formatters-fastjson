/*
 * Author: Aliqi
 * E-mail: aliqi@hotmail.com
 * Created Time: 2011-03-12
 * Copyright: If you want to use this module, please retain this comment.
 * You can change any code of this file and add your name to the developers list,
 * but you cannot delete this comment or modify all content above.
 * Here's the Developers List:
 */

using System;

namespace DragonScale.Portable
{
    /// <summary>
    /// Defines the EventHandler for the first-level member in event model.
    /// </summary>
    /// <typeparam name="TSender"></typeparam>
    /// <typeparam name="TEventArgs"></typeparam>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void EventHandler<TSender, TEventArgs>(TSender sender, TEventArgs e)
        where TEventArgs : EventArgs;
}
