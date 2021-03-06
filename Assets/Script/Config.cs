﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class Config : MonoBehaviour
{
	public int DefaultFontSize = 14;
	public float DefaultWidthPerLevel = 27;
	public float DefaultHeightPerLine = 27.0f;
	
	//public bool DoBackUp = true;

	public int FontSize = 14;
	public float WidthFactor = 1.0f;
	public float HeightFactor = 1.0f;
	public float WidthPerLevel { get { return DefaultWidthPerLevel * WidthFactor * (float)FontSize / DefaultFontSize; } }
	public float HeightPerLine { get { return DefaultHeightPerLine * HeightFactor * (float)FontSize / DefaultFontSize; } }
	public float AnimTime = 0.05f;
	public float AnimOvershoot = 1.70158f;
	public float ArrowStreamDelayTime = 0.3f;
	public float ArrowStreamIntervalTime = 0.03f;
	public float TextInputFixIntervalTime = 1.0f;
	public float DoubleClickInterval = 0.25f;
	public float MinLogTreeHeight = 100.0f;
	public float AutoSaveTime = 1.5f;
	public int LogLoadUnit = 7;
	public int NumRecentFilesMenu = 9;

	public float TagLineHeight = 30;
	public float TagListWidth = 200;
	public float TagCommaInterval = 58;
	public float LogNoteHeaderMargin = 30;

	public Color ThemeColor;
	public Color AccentColor;
	public Color DoneColor;
	public Color DiaryColor;

	public Color SelectionColor;
	public Color TextColor;
	public Color StrikeColor;
	public Color DoneTextColor;
	public Color CloneTextColor;
	public Color CommentLineColor;
	public Color CommentTextColor;
	public Color TagSelectionColor;
	public Color TagSubTextColor;

	public Color ToggleColor;
	public Color ToggleOpenedColor;

	public string TimeFormat = "HH:mm";
	public string DateFormat = "yyyy/M/d (ddd)";
	public string DefaultTag = "todo";

	FileInfo configFile_;

	// Use this for initialization
	void Awake()
	{
		GameContext.Config = this;
		configFile_ = new FileInfo(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Dones/config.txt");
		
		LoadConfig();
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnValidate()
	{
		AnimInfoBase.overshoot = AnimOvershoot;
	}

	void OnApplicationQuit()
	{
		SaveConfig();
	}

	#region config save / load

	enum ConfigParams
	{
		TimeFormat,
		DateFormat,
		FontSize,
		Count
	}
	static string[] ConfigTags = new string[(int)ConfigParams.Count] {
		"[TimeFormat]",
		"[DateFormat]",
		"[FontSize]",
	};
	void LoadConfig()
	{
		if( configFile_.Exists == false )
		{
			return;
		}

		StreamReader reader = new StreamReader(configFile_.OpenRead());
		string text = null;

		ConfigParams configParam = ConfigParams.TimeFormat;
		while( (text = reader.ReadLine()) != null )
		{
			foreach( ConfigParams param in (ConfigParams[])Enum.GetValues(typeof(ConfigParams)) )
			{
				if( param == ConfigParams.Count ) break;
				else if( ConfigTags[(int)param] == text )
				{
					configParam = param;
					break;
				}
			}
			text = reader.ReadLine();
			while( text.StartsWith("//") )
			{
				text = reader.ReadLine();
			}
			switch( configParam )
			{
			case ConfigParams.TimeFormat:
				TimeFormat = text;
				break;
			case ConfigParams.DateFormat:
				DateFormat = text;
				break;
			case ConfigParams.FontSize:
				int.TryParse(text, out FontSize);
				break;
			}
		}

		reader.Close();
	}

	void SaveConfig()
	{
		if( configFile_.Exists == false )
		{
			if( Directory.Exists(configFile_.DirectoryName) == false )
			{
				Directory.CreateDirectory(configFile_.DirectoryName);
			}
		}

		StreamWriter writer = new StreamWriter(configFile_.FullName);
		
		writer.WriteLine(ConfigTags[(int)ConfigParams.TimeFormat]);
		writer.WriteLine(TimeFormat);
		writer.WriteLine(ConfigTags[(int)ConfigParams.DateFormat]);
		writer.WriteLine(DateFormat);
		writer.WriteLine(ConfigTags[(int)ConfigParams.FontSize]);
		writer.WriteLine(FontSize);

		writer.Flush();
		writer.Close();
	}

	#endregion
}
