﻿/*
    Copyright (C) 2014-2015 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using dnlib.DotNet;

namespace dnSpy.AsmEditor.DnlibDialogs
{
	enum CharSet
	{
		NotSpec		= (int)PInvokeAttributes.CharSetNotSpec >> 1,
		Ansi		= (int)PInvokeAttributes.CharSetAnsi >> 1,
		Unicode		= (int)PInvokeAttributes.CharSetUnicode >> 1,
		Auto		= (int)PInvokeAttributes.CharSetAuto >> 1,
	}

	enum BestFit
	{
		UseAssem	= (int)PInvokeAttributes.BestFitUseAssem >> 4,
		Enabled		= (int)PInvokeAttributes.BestFitEnabled >> 4,
		Disabled	= (int)PInvokeAttributes.BestFitDisabled >> 4,
	}

	enum ThrowOnUnmappableChar
	{
		UseAssem	= (int)PInvokeAttributes.ThrowOnUnmappableCharUseAssem >> 12,
		Enabled		= (int)PInvokeAttributes.ThrowOnUnmappableCharEnabled >> 12,
		Disabled	= (int)PInvokeAttributes.ThrowOnUnmappableCharDisabled >> 12,
	}

	enum CallConv
	{
		Winapi		= (int)PInvokeAttributes.CallConvWinapi >> 8,
		Cdecl		= (int)PInvokeAttributes.CallConvCdecl >> 8,
		Stdcall		= (int)PInvokeAttributes.CallConvStdcall >> 8,
		Thiscall	= (int)PInvokeAttributes.CallConvThiscall >> 8,
		Fastcall	= (int)PInvokeAttributes.CallConvFastcall >> 8,
	}

	sealed class ImplMapVM : ViewModelBase {
		static readonly EnumVM[] charSetList = EnumVM.Create(typeof(DnlibDialogs.CharSet));
		public EnumListVM CharSet {
			get { return charSetVM; }
		}
		readonly EnumListVM charSetVM = new EnumListVM(charSetList);

		static readonly EnumVM[] bestFitList = EnumVM.Create(typeof(DnlibDialogs.BestFit));
		public EnumListVM BestFit {
			get { return bestFitVM; }
		}
		readonly EnumListVM bestFitVM = new EnumListVM(bestFitList);

		static readonly EnumVM[] throwOnUnmappableCharList = EnumVM.Create(typeof(DnlibDialogs.ThrowOnUnmappableChar));
		public EnumListVM ThrowOnUnmappableChar {
			get { return throwOnUnmappableCharVM; }
		}
		readonly EnumListVM throwOnUnmappableCharVM = new EnumListVM(throwOnUnmappableCharList);

		static readonly EnumVM[] callConvList = EnumVM.Create(typeof(DnlibDialogs.CallConv));
		public EnumListVM CallConv {
			get { return callConvVM; }
		}
		readonly EnumListVM callConvVM = new EnumListVM(callConvList);

		public bool IsEnabled {
			get { return isEnabled; }
			set {
				if (isEnabled != value) {
					isEnabled = value;
					OnPropertyChanged("IsEnabled");
				}
			}
		}
		bool isEnabled = true;

		public string Name {
			get { return name; }
			set {
				if (name != value) {
					name = value;
					OnPropertyChanged("Name");
				}
			}
		}
		string name = string.Empty;

		public string ModuleName {
			get { return moduleName; }
			set {
				if (moduleName != value) {
					moduleName = value;
					OnPropertyChanged("ModuleName");
				}
			}
		}
		string moduleName = string.Empty;

		public PInvokeAttributes Attributes {
			get {
				var mask = PInvokeAttributes.CharSetMask |
							PInvokeAttributes.BestFitMask |
							PInvokeAttributes.ThrowOnUnmappableCharMask |
							PInvokeAttributes.CallConvMask;
				return (attributes & ~mask) |
					(PInvokeAttributes)((int)(DnlibDialogs.CharSet)CharSet.SelectedItem << 1) |
					(PInvokeAttributes)((int)(DnlibDialogs.BestFit)BestFit.SelectedItem << 4) |
					(PInvokeAttributes)((int)(DnlibDialogs.ThrowOnUnmappableChar)ThrowOnUnmappableChar.SelectedItem << 12) |
					(PInvokeAttributes)((int)(DnlibDialogs.CallConv)CallConv.SelectedItem << 8);
			}
			set {
				if (attributes != value) {
					attributes = value;
					OnPropertyChanged("Attributes");
					OnPropertyChanged("NoMangle");
					OnPropertyChanged("SupportsLastError");
				}
			}
		}
		PInvokeAttributes attributes;

		public bool NoMangle {
			get { return GetFlagValue(PInvokeAttributes.NoMangle); }
			set { SetFlagValue(PInvokeAttributes.NoMangle, value); }
		}

		public bool SupportsLastError {
			get { return GetFlagValue(PInvokeAttributes.SupportsLastError); }
			set { SetFlagValue(PInvokeAttributes.SupportsLastError, value); }
		}

		bool GetFlagValue(PInvokeAttributes flag) {
			return (Attributes & flag) != 0;
		}

		void SetFlagValue(PInvokeAttributes flag, bool value) {
			if (value)
				Attributes |= flag;
			else
				Attributes &= ~flag;
		}

		public ImplMap ImplMap {
			get {
				if (!IsEnabled)
					return null;
				ModuleRef modRef = ModuleName == null ? null : ownerModule.UpdateRowId(new ModuleRefUser(ownerModule, ModuleName));
				return ownerModule.UpdateRowId(new ImplMapUser(modRef, Name, Attributes));
			}
			set {
				IsEnabled = value != null;
				if (value == null)
					return;

				Name = value.Name;
				Attributes = value.Attributes;
				ModuleName = value.Module == null ? null : value.Module.Name;
				CharSet.SelectedItem = (DnlibDialogs.CharSet)((int)(value.Attributes & PInvokeAttributes.CharSetMask) >> 1);
				BestFit.SelectedItem = (DnlibDialogs.BestFit)((int)(value.Attributes & PInvokeAttributes.BestFitMask) >> 4);
				ThrowOnUnmappableChar.SelectedItem = (DnlibDialogs.ThrowOnUnmappableChar)((int)(value.Attributes & PInvokeAttributes.ThrowOnUnmappableCharMask) >> 12);
				CallConv.SelectedItem = (DnlibDialogs.CallConv)((int)(value.Attributes & PInvokeAttributes.CallConvMask) >> 8);
			}
		}

		readonly ModuleDef ownerModule;

		public ImplMapVM(ModuleDef ownerModule) {
			this.ownerModule = ownerModule;
		}
	}
}
