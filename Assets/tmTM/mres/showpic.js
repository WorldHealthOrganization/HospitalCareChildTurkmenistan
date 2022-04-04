var picshown=false;

var ios = navigator.platform.indexOf("iPhone") >= 0 ||
	navigator.platform.indexOf("iPod") >= 0;
var iPad = navigator.platform.indexOf("iPad") >= 0;

function f_filterResults(n_win, n_docel, n_body)
{
	var n_result = n_win ? n_win : 0;
	if (n_docel && (!n_result || (n_result > n_docel)))
		n_result = n_docel;
	return n_body && (!n_result || (n_result > n_body)) ? n_body : n_result;
}

function f_clientWidth()
{
	var winW = f_filterResults (
		window.innerWidth ? window.innerWidth : 0,
		document.documentElement ? document.documentElement.clientWidth : 0,
		document.body ? document.body.clientWidth : 0);
	
	if (ios || iPad)
	{
		switch(window.orientation) 
		{
		case -90:
		case 90:
			winW = screen.availHeight;
			break;
		default:
			winW = screen.availWidth;
			break; 
		}
	}
	return winW;
}

function f_clientHeight()
{
	var winH = f_filterResults (
		window.innerHeight ? window.innerHeight : 0,
		document.documentElement ? document.documentElement.clientHeight : 0,
		document.body ? document.body.clientHeight : 0);
	
	if (ios || iPad)
	{
		switch(window.orientation) 
		{
		case -90:
		case 90:
			winH = screen.availWidth;
			break; 
		default:
			winH = screen.availHeight;
			break; 
		}
	}
	if (ios) {
		winH = winH - 120;
		var iHeight = window.screen.height;
		var iWidth = window.screen.width;
		if (!((iHeight == 667 && iWidth == 375) || (iHeight == 736 && iWidth == 414))) {
			winH = winH - 60;
		}
	}
	return winH;
}

function titleOnTop()
{
	var headerElt = document.getElementById("header");
	if(headerElt == null) headerElt = document.getElementById("headeri");
	var uheadElt = document.getElementById("uhead");
	//alert('headerElt.style.zIndex = '+headerElt.style.zIndex+', uheadElt.style.zIndex = '+uheadElt.style.zIndex);
	if(headerElt != null && uheadElt != null) headerElt.style.zIndex = uheadElt.style.zIndex + 3;
	//if(headerElt != null) headerElt.style.visibility = "visible";
	/*if(headerElt != null)
	{
		headerElt.style.top = 0;
		headerElt.style.right = 0;
		headerElt.style.left = 0;
		alert('top = '+headerElt.style.top+
		', right = '+headerElt.style.right+
		', left = '+headerElt.style.left);
	}*/
}

function showpic(url, w, h, text)
{
	if(picshown) return;
	picshown=true;
	
	var headerElt = document.getElementById("header");
	if(headerElt == null) headerElt = document.getElementById("headeri");
	
	var bigpicw = document.createElement("div");
	if(headerElt != null) bigpicw.style.zIndex = headerElt.style.zIndex + 1;
	bigpicw.setAttribute("id", "bigpicw");
	bigpicw.setAttribute("onclick", "hidepic();");
	
	var bigpic = document.createElement("img");
	bigpic.setAttribute("id", "bigpic");
	bigpic.setAttribute("src", url);
	bigpicw.appendChild(bigpic);
	
	
	
	document.body.appendChild(bigpicw);
	
	var ww = f_clientWidth(); //$('body').width();
	var wh = f_clientHeight() - 90; //$('body').height();
	
	if(w > ww)
	{
		h = h * ww / w;
		w = ww;
	}
	if(h > wh)
	{
		w = w * wh /h;
		h = wh;
	}
	
	//var t = wh / 2 - h / 2;
	//var l = ww / 2 - w / 2;
	
	bigpic.width = w
	bigpic.height = h
	bigpicw.width = w
	bigpicw.height = h
	
	// test of adding text to the picture:
	if(text.length > 0)
	{
	var capt = document.createTextNode(text);
	var captp = document.createElement("p");
	captp.setAttribute("class", "body");
	captp.appendChild(capt);
	//alert("bigpicw.width = " + bigpicw.width);
	bigpicw.appendChild(captp);//*/
	//alert("bigpicw.width = " + bigpicw.width);
	//alert("bigpicw.clientWidth = " + bigpicw.clientWidth);
	
	if(bigpicw.clientHeight > wh)
	{
		var nh = wh - captp.clientHeight;
		bigpic.width = w * nh / h;
		bigpic.height = nh;
		bigpicw.height = wh;
	}
	
	bigpicw.style.left = ww / 2 - bigpicw.clientWidth / 2;
	bigpicw.style.top = wh / 2 - bigpicw.clientHeight / 2;
	}
	else
	{
	bigpicw.style.left = ww / 2 - w / 2;
	bigpicw.style.top = wh / 2 - h / 2;
	}
}

function shownote(text)
{
	if(picshown) return;
	picshown=true;
	
	var headerElt = document.getElementById("header");
	if(headerElt == null) headerElt = document.getElementById("headeri");
	
	var bigpicw = document.createElement("div");
	if(headerElt != null) bigpicw.style.zIndex = headerElt.style.zIndex + 1;
	bigpicw.setAttribute("id", "bigpicw");
	bigpicw.setAttribute("onclick", "hidepic();");
	
	var arr = text.split('\n');

	for(let i=0; i<arr.length; i++)
	{
		var capt = document.createTextNode(arr[i]);
		var captp = document.createElement("p");
		captp.setAttribute("class", "body");
		captp.appendChild(capt);
		bigpicw.appendChild(captp);
	}

	
	document.body.appendChild(bigpicw);
	
	var wh = f_clientHeight();
	var ww = f_clientWidth();
	
	//bigpicw.style.left = 0;
	bigpicw.style.left = ww / 2 - bigpicw.clientWidth / 2;
	bigpicw.style.top = wh / 2 - bigpicw.clientHeight / 2;
}

function hidepic()
{
	//alert("bigpicw.width = " + bigpicw.width);
	document.getElementById("bigpicw").remove();
	picshown=false;
}

function settextsize()
{
	if (localStorage && 'untsstr' in localStorage)
	{
		document.getElementById("bco").style.fontSize = localStorage.untsstr;
	}
}

var onloadwidth = f_clientWidth();

function gotresize()
{
	if(onloadwidth != f_clientWidth())
		location.reload(false);
}

function zoom(id)
{
	
	var element = document.getElementById(id);
	if(id=="sh2")
	{
		panzoom(element, {
			maxZoom: 3,
			minZoom: 1,
			bounds: true,
			  boundsPadding: 1,
			paddingTop: 0.3
			});
	}
	else
	{
    panzoom(element, {
    maxZoom: 3,
	minZoom: 1,
	bounds: true,
  	boundsPadding: 1,
	paddingTop: 0.5
    });
}
}