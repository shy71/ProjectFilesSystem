// Copyright (c) 2011 by HHD Software Ltd.
// This file is part of the HHD Software Hex Editor Neo
// For usage and distribution policies, consult the license distributed with a product installation program

// Hex Editor Neo's Structure Viewer sample declaration file

#pragma once

#include "bitmap.h"

//
// For resource directory entries that have actual string names, the Name
// field of the directory entry points to an object of the following type.
// All of these string objects are stored together after the last resource
// directory entry and before the first resource data object.  This minimizes
// the impact of these variable length objects on the alignment of the fixed
// size directory entry objects.
struct IMAGE_RESOURCE_DIRECTORY_STRING 
{
    string str;
};


struct IMAGE_RESOURCE_DIR_STRING_U 
{
    wstring str;
};


// forward declaration
struct IMAGE_RESOURCE_DIRECTORY;

// Already defined RESOURCE types (index to string)
const ResourceTypes[] = 
{
	"???_0", 
	"CURSOR", 
	"BITMAP", 
	"ICON", 
	"MENU", 
	"DIALOG", 
	"STRING", 
	"FONTDIR",
	"FONT", 
	"ACCELERATORS", 
	"RCDATA", 
	"MESSAGETABLE", 
	"GROUP_CURSOR",
	"???_13", 
	"GROUP_ICON", 
	"???_15", 
	"VERSION",
	"???_17", 
	"???_18",
	"???_19",
	"???_20",
	"???_21",
	"???_22",
	"???_23",
	"MANIFEST" 
};

// special handling of MANIFEST and STRING resources
struct RESOURCE_DATA
{
	switch(CurrentResourceType)
	{
	case 6:		// STRING
	case 24:	// MANIFEST
		char data[Size];
		break;
	default:
		BYTE data[Size];
	}
};

struct IMAGE_RESOURCE_DATA_ENTRY 
{
	union
	{
		DWORD   OffsetToData;
		if(OffsetToData)
		{
			if(CurrentResourceType==2)
			{
				// bitmap (using bitmap.h file)
				DWORD Data as BITMAPFILEHEADER*(int(RvaToPaBiased(OffsetToData)));
			}else	// generic
				DWORD Data as RESOURCE_DATA*(int(RvaToPaBiased(OffsetToData)));
		}
	}Data;

    DWORD   Size;

    DWORD   CodePage;
    DWORD   Reserved;
};

// FD
struct IMAGE_RESOURCE_DIRECTORY_selector;

//
// Each directory contains the 32-bit Name of the entry and an offset,
// relative to the beginning of the resource directory of the data associated
// with this directory entry.  If the name of the entry is an actual text
// string instead of an integer Id, then the high order bit of the name field
// is set to one and the low order 31-bits are an offset, relative to the
// beginning of the resource directory of the string, which is of type
// IMAGE_RESOURCE_DIRECTORY_STRING.  Otherwise the high bit is clear and the
// low-order 16-bits are the integer Id that identify this resource directory
// entry. If the directory entry is yet another resource directory (i.e. a
// subdirectory), then the high order bit of the offset field will be
// set to indicate this.  Otherwise the high bit is clear and the offset
// field points to a resource data entry.
public struct IMAGE_RESOURCE_DIRECTORY_ENTRY 
{		
	var virtual_ro = int(GetDataDirectoryOffset(IMAGE_DIRECTORY_ENTRY_RESOURCE));

	if(root_level)				// set in IMAGE_RESOURCE_DIRECTORY_ROOT/IMAGE_RESOURCE_DIRECTORY
		var CurrentResourceType = 0;

	union 
	{
		// this field(s) is made hidden because we have to check NameIsString flag
		// but to use current offset (NameOffset) for NameString. That is why we use union.
hidden:
		struct 
		{
			DWORD NameOffset:31;
			DWORD NameIsString:1;
		}Name_;

visible:
		if(Name_.NameIsString)
		{
			// string relative to start of the resources
			DWORD NameString as WTStringWrapper* (RvaToPa(virtual_ro + (Name_.NameOffset)) - NameString);
		}else
		{
			// resource name is an index into the table
			DWORD Id;
			if(root_level)
			{
				if(Id<25)
					CurrentResourceType = Id;
				else
					CurrentResourceType = 0;

				$print("ResourceType",ResourceTypes[Id]);
			}
		}
	}Name;

	union 
	{
hidden:
		// this field(s) is made hidden because we have to check DataIsDirectory flag
		// but to use current offset (OffsetToDirectory) for ChildDataDirectory/ChildDataEntry. That is why we use union
		struct 
		{
			DWORD   OffsetToDirectory:31;
			DWORD   DataIsDirectory:1;
		} Offset_;
visible:		
		if(Offset_.OffsetToDirectory)
		{
			if(Offset_.DataIsDirectory)
			{
				DWORD ChildDataDirectory as IMAGE_RESOURCE_DIRECTORY* (RvaToPa(virtual_ro + (Offset_.OffsetToDirectory)) - ChildDataDirectory);
			}else
			{
				DWORD ChildDataEntry as IMAGE_RESOURCE_DATA_ENTRY* (RvaToPa(virtual_ro + (Offset_.OffsetToDirectory)) - ChildDataEntry);
			}
		}
	}Offset;
};

//
// Resource directory consists of two counts, following by a variable length
// array of directory entries.  The first count is the number of entries at
// beginning of the array that have actual names associated with each entry.
// The entries are in ascending order, case insensitive strings.  The second
// count is the number of entries that immediately follow the named entries.
// This second count identifies the number of entries that have 16-bit integer
// Ids as their name.  These entries are also sorted in ascending order.
//
// This structure allows fast lookup by either name or number, but for any
// given resource entry only one form of lookup is supported, not both.
// This is consistant with the syntax of the .RC file and the .RES file.
//
public struct IMAGE_RESOURCE_DIRECTORY_ROOT 
{
	var root_level = true;				// root level means that we have nested resource dirctories

	DWORD   Characteristics;
    DWORD   TimeDateStamp;
    WORD    MajorVersion;
    WORD    MinorVersion;
    WORD    NumberOfNamedEntries;
    WORD    NumberOfIdEntries;

	[noindex] IMAGE_RESOURCE_DIRECTORY_ENTRY DirectoryEntries[NumberOfNamedEntries + NumberOfIdEntries];
};

struct IMAGE_RESOURCE_DIRECTORY 
{
	root_level = false;					// root level means that we have nested resource dirctories

	DWORD   Characteristics;
    DWORD   TimeDateStamp;
    WORD    MajorVersion;
    WORD    MinorVersion;
    WORD    NumberOfNamedEntries;
    WORD    NumberOfIdEntries;

	[noindex] IMAGE_RESOURCE_DIRECTORY_ENTRY DirectoryEntries[NumberOfNamedEntries + NumberOfIdEntries];
};