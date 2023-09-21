/* -LICENSE-START-
** Copyright (c) 2018 Blackmagic Design
**
** Permission is hereby granted, free of charge, to any person or organization
** obtaining a copy of the software and accompanying documentation covered by
** this license (the "Software") to use, reproduce, display, distribute,
** execute, and transmit the Software, and to prepare derivative works of the
** Software, and to permit third-parties to whom the Software is furnished to
** do so, all subject to the following:
** 
** The copyright notices in the Software and this entire statement, including
** the above license grant, this restriction and the following disclaimer,
** must be included in all copies of the Software, in whole or in part, and
** all derivative works of the Software, unless such copies or derivative
** works are solely in the form of machine-executable object code generated by
** a source language processor.
** 
** THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
** IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
** FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT
** SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE
** FOR ANY DAMAGES OR OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE,
** ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
** DEALINGS IN THE SOFTWARE.
** -LICENSE-END-
*/
#include "stdafx.h"
#include "platform.h"
#include "Bgra32VideoFrame.h"

/* Bgra32VideoFrame class */

// Constructor generates empty pixel buffer
Bgra32VideoFrame::Bgra32VideoFrame(long width, long height, BMDFrameFlags flags) : 
	m_width(width), m_height(height), m_flags(flags), m_refCount(1)
{
	// Allocate pixel buffer
	m_pixelBuffer.resize(m_width*m_height*4);
}

HRESULT Bgra32VideoFrame::GetBytes(void **buffer)
{
	*buffer = (void*)m_pixelBuffer.data();
	return S_OK;
}

HRESULT	STDMETHODCALLTYPE Bgra32VideoFrame::QueryInterface(REFIID iid, LPVOID *ppv)
{
	HRESULT 		result = E_NOINTERFACE;

	if (ppv == NULL)
		return E_INVALIDARG;

	// Initialise the return result
	*ppv = NULL;

	// Obtain the IUnknown interface and compare it the provided REFIID
	if (iid == IID_IUnknown)
	{
		*ppv = this;
		AddRef();
		result = S_OK;
	}
	
	else if (iid == IID_IDeckLinkVideoFrame)
	{
		*ppv = (IDeckLinkVideoFrame*)this;
		AddRef();
		result = S_OK;
	}

	return result;
}

ULONG STDMETHODCALLTYPE Bgra32VideoFrame::AddRef(void)
{
	return ++m_refCount;
}

ULONG STDMETHODCALLTYPE Bgra32VideoFrame::Release(void)
{

	ULONG newRefValue = --m_refCount;
	if (newRefValue == 0)
		delete this;

	return newRefValue;
}
