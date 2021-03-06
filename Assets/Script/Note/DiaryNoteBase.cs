﻿using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// Window - [ LogNote ] - LogTree - Line
public class DiaryNoteBase : Note
{
	protected static Dictionary<string, LogTree> EditedLogTreeDict = new Dictionary<string, LogTree>();

	public bool IsEdited
	{
		get
		{
			foreach( LogTree logTree in logTrees_ )
			{
				if( logTree.IsEdited )
					return true;
			}
			return false;
		}
	}

	protected List<LogTree> logTrees_ = new List<LogTree>();
	protected DateTime today_;
	protected DateTime endDate_;
	protected bool endLoad_ = false;

	protected override void Awake()
	{
		base.Awake();
		today_ = DateTime.Now.Date;
		endDate_ = today_;
	}

	protected override void Update()
	{
		base.Update();

		if( endLoad_ == false && scrollRect_.verticalScrollbar.isActiveAndEnabled && scrollRect_.verticalScrollbar.value <= 100.0f / layout_.preferredHeight )
		{
			LoadMore();
		}
	}
	
	
	public void SubscribeKeyInput()
	{
		foreach( LogTree logTree in logTrees_ )
		{
			logTree.SubscribeKeyInput();
		}
	}

	public void OnOverflowArrowInput(LogTree tree, KeyCode key)
	{
		int index = logTrees_.IndexOf(tree);
		int nextIndex = index + 1;
		while( nextIndex < logTrees_.Count - 1 && logTrees_[nextIndex].gameObject.activeInHierarchy == false )
		{
			++nextIndex;
		}
		int prevIndex = index - 1;
		while( prevIndex > 0 && logTrees_[prevIndex].gameObject.activeInHierarchy == false )
		{
			--prevIndex;
		}
		switch(key)
		{
		case KeyCode.DownArrow:
			{
				Line next = (nextIndex < logTrees_.Count ? logTrees_[nextIndex].RootLine[0] : null);
				if( next != null )
				{
					next.Field.IsFocused = true;
				}
			}
			break;
		case KeyCode.UpArrow:
			{
				Line prev = (prevIndex >= 0 ? logTrees_[prevIndex].RootLine.LastVisibleLine : null);
				if( prev != null )
				{
					prev.Field.IsFocused = true;
				}
			}
			break;
		case KeyCode.RightArrow:
			{
				Line next = (nextIndex < logTrees_.Count ? logTrees_[nextIndex].RootLine[0] : null);
				if( next != null )
				{
					next.Field.CaretPosision = 0;
					next.Field.IsFocused = true;
				}
			}
			break;
		case KeyCode.LeftArrow:
			{
				Line prev = (prevIndex >= 0 ? logTrees_[prevIndex].RootLine.LastVisibleLine : null);
				if( prev != null )
				{
					prev.Field.CaretPosision = prev.TextLength;
					prev.Field.IsFocused = true;
				}
			}
			break;
		}
	}
	
	public void OnHomeEndInput(KeyCode key)
	{
		if( logTrees_.Count > 0 )
		{
			if( key == KeyCode.Home )
			{
				Line line = logTrees_[0].RootLine[0];
				line.Field.IsFocused = true;
				logTrees_[0].OnFocused(line);
			}
			else if( key == KeyCode.End )
			{
				Line line = logTrees_[logTrees_.Count - 1].RootLine.LastVisibleLine;
				line.Field.IsFocused = true;
				logTrees_[logTrees_.Count - 1].OnFocused(line);
			}
		}
	}
	
	public void OnFontSizeChanged()
	{
		foreach( LogTree logTree in logTrees_ )
		{
			logTree.RootLine.AdjustFontSizeRecursive(GameContext.Config.FontSize, GameContext.Config.HeightPerLine);
			logTree.UpdateLayoutElement();
		}
		if( gameObject.activeInHierarchy )
		{
			UpdateLayoutElement();
		}
	}


	#region file

	public virtual void LoadMore()
	{
		endLoad_ = true;
	}

	#endregion


	public static Color ToColor(DateTime date)
	{
		if( date.Date == DateTime.Now.Date ) return GameContext.Config.DoneColor;
		else if( date.DayOfWeek == DayOfWeek.Sunday ) return GameContext.Config.AccentColor;
		else if( date.DayOfWeek == DayOfWeek.Saturday ) return GameContext.Config.AccentColor;
		else return GameContext.Config.TextColor;
	}

	public static string ToDirectoryName(TreeNote treeNote)
	{
		return String.Format("{0}/{1}.dones/", treeNote.File.DirectoryName, treeNote.File.Name.Replace(".dtml", ""));
	}

	public static string ToFileName(TreeNote treeNote, DateTime date)
	{
		return String.Format("{0}/{1}.dones/{1}{2}.dtml", treeNote.File.DirectoryName, treeNote.File.Name.Replace(".dtml", ""), date.ToString("-yyyy-MM-dd"));
	}
}