/*
JSFromHell.com @ http://jsfromhell.com
--------------------------------------

Name: Big Number
URL: http://jsfromhell.com/classes/bignumber
Author: Jonas Raoni Soares Silva
Created: 2006.05.03
Modified: 2009.07.16
Description:
Offers a extremely high precision level to make mathematical operations. For integers there is no limits and for floating point numbers, the class allows setting the maximum precision.

To keep yourself updated, sign up the ATOM feed at: http://jsfromhell.com/en/feed/atom.xml

We authorize the copy and modification of all the codes on the site, since the original author credits are kept
In the case you have a code that you consider indispensable, job offers, feature requests, suggestions or if you've found problems on the site, contact us.
We don't assume any responsibility for any kind of damage, direct or indirect, that can be raised upon the utilization of our codes/site or any site that's available through hiperlinks in our site.
*/

//+ Jonas Raoni Soares Silva
//@ http://jsfromhell.com/classes/bignumber [rev. #4]

BigNumber = function (n, p, r) {
    var o = this, i;
    if (n instanceof BigNumber) {
        for (i in { precision: 0, roundType: 0, _s: 0, _f: 0 }) o[i] = n[i];
        o._d = n._d.slice();
        return;
    }
    o.precision = isNaN(p = Math.abs(p)) ? BigNumber.defaultPrecision : p;
    o.roundType = isNaN(r = Math.abs(r)) ? BigNumber.defaultRoundType : r;
    o._s = (n += "").charAt(0) == "-";
    o._f = ((n = n.replace(/[^\d.]/g, "").split(".", 2))[0] = n[0].replace(/^0+/, "") || "0").length;
    for (i = (n = o._d = (n.join("") || "0").split("")).length; i; n[--i] = +n[i]);
    o.round();
};
with({$: BigNumber, o: BigNumber.prototype}){
	$.ROUND_HALF_EVEN = ($.ROUND_HALF_DOWN = ($.ROUND_HALF_UP = ($.ROUND_FLOOR = ($.ROUND_CEIL = ($.ROUND_DOWN = ($.ROUND_UP = 0) + 1) + 1) + 1) + 1) + 1) + 1;
	$.defaultPrecision = 40;
	$.defaultRoundType = $.ROUND_HALF_UP;
	o.add = function(n){
		if(this._s != (n = new BigNumber(n))._s)
			return n._s ^= 1, this.subtract(n);
		var o = new BigNumber(this), a = o._d, b = n._d, la = o._f,
		lb = n._f, n = Math.max(la, lb), i, r;
		la != lb && ((lb = la - lb) > 0 ? o._zeroes(b, lb, 1) : o._zeroes(a, -lb, 1));
		i = (la = a.length) == (lb = b.length) ? a.length : ((lb = la - lb) > 0 ? o._zeroes(b, lb) : o._zeroes(a, -lb)).length;
		for(r = 0; i; r = (a[--i] = a[i] + b[i] + r) / 10 >>> 0, a[i] %= 10);
		return r && ++n && a.unshift(r), o._f = n, o.round();
	};
	o.subtract = function(n){
		if(this._s != (n = new BigNumber(n))._s)
			return n._s ^= 1, this.add(n);
		var o = new BigNumber(this), c = o.abs().compare(n.abs()) + 1, a = c ? o : n, b = c ? n : o, la = a._f, lb = b._f, d = la, i, j;
		a = a._d, b = b._d, la != lb && ((lb = la - lb) > 0 ? o._zeroes(b, lb, 1) : o._zeroes(a, -lb, 1));
		for(i = (la = a.length) == (lb = b.length) ? a.length : ((lb = la - lb) > 0 ? o._zeroes(b, lb) : o._zeroes(a, -lb)).length; i;){
			if(a[--i] < b[i]){
				for(j = i; j && !a[--j]; a[j] = 9);
				--a[j], a[i] += 10;
			}
			b[i] = a[i] - b[i];
		}
		return c || (o._s ^= 1), o._f = d, o._d = b, o.round();
	};
	o.multiply = function(n){
		var o = new BigNumber(this), r = o._d.length >= (n = new BigNumber(n))._d.length, a = (r ? o : n)._d,
		b = (r ? n : o)._d, la = a.length, lb = b.length, x = new BigNumber, i, j, s;
		for(i = lb; i; r && s.unshift(r), x.set(x.add(new BigNumber(s.join("")))))
			for(s = (new Array(lb - --i)).join("0").split(""), r = 0, j = la; j; r += a[--j] * b[i], s.unshift(r % 10), r = (r / 10) >>> 0);
		return o._s = o._s != n._s, o._f = ((r = la + lb - o._f - n._f) >= (j = (o._d = x._d).length) ? this._zeroes(o._d, r - j + 1, 1).length : j) - r, o.round();
	};
	o.divide = function(n){
		if((n = new BigNumber(n)) == "0")
			throw new Error("Division by 0");
		else if(this == "0")
			return new BigNumber;
		var o = new BigNumber(this), a = o._d, b = n._d, la = a.length - o._f,
		lb = b.length - n._f, r = new BigNumber, i = 0, j, s, l, f = 1, c = 0, e = 0;
		r._s = o._s != n._s, r.precision = Math.max(o.precision, n.precision),
		r._f = +r._d.pop(), la != lb && o._zeroes(la > lb ? b : a, Math.abs(la - lb));
		n._f = b.length, b = n, b._s = false, b = b.round();
		for(n = new BigNumber; a[0] == "0"; a.shift());
		out:
		do{
			for(l = c = 0, n == "0" && (n._d = [], n._f = 0); i < a.length && n.compare(b) == -1; ++i){
				(l = i + 1 == a.length, (!f && ++c > 1 || (e = l && n == "0" && a[i] == "0")))
				&& (r._f == r._d.length && ++r._f, r._d.push(0));
				(a[i] == "0" && n == "0") || (n._d.push(a[i]), ++n._f);
				if(e)
					break out;
				if((l && n.compare(b) == -1 && (r._f == r._d.length && ++r._f, 1)) || (l = 0))
					while(r._d.push(0), n._d.push(0), ++n._f, n.compare(b) == -1);
			}
			if(f = 0, n.compare(b) == -1 && !(l = 0))
				while(l ? r._d.push(0) : l = 1, n._d.push(0), ++n._f, n.compare(b) == -1);
			for(s = new BigNumber, j = 0; n.compare(y = s.add(b)) + 1 && ++j; s.set(y));
			n.set(n.subtract(s)), !l && r._f == r._d.length && ++r._f, r._d.push(j);
		}
		while((i < a.length || n != "0") && (r._d.length - r._f) <= r.precision);
		return r.round();
	};
	o.mod = function(n){
		return this.subtract(this.divide(n).intPart().multiply(n));
	};
	o.pow = function(n){
		var o = new BigNumber(this), i;
		if((n = (new BigNumber(n)).intPart()) == 0) return o.set(1);
		for(i = Math.abs(n); --i; o.set(o.multiply(this)));
		return n < 0 ? o.set((new BigNumber(1)).divide(o)) : o;
	};
	o.set = function(n){
		return this.constructor(n), this;
	};
	o.compare = function(n){
		var a = this, la = this._f, b = new BigNumber(n), lb = b._f, r = [-1, 1], i, l;
		if(a._s != b._s)
			return a._s ? -1 : 1;
		if(la != lb)
			return r[(la > lb) ^ a._s];
		for(la = (a = a._d).length, lb = (b = b._d).length, i = -1, l = Math.min(la, lb); ++i < l;)
			if(a[i] != b[i])
				return r[(a[i] > b[i]) ^ a._s];
		return la != lb ? r[(la > lb) ^ a._s] : 0;
	};
	o.negate = function(){
		var n = new BigNumber(this); return n._s ^= 1, n;
	};
	o.abs = function(){
		var n = new BigNumber(this); return n._s = 0, n;
	};
	o.intPart = function(){
		return new BigNumber((this._s ? "-" : "") + (this._d.slice(0, this._f).join("") || "0"));
	};
	o.valueOf = o.toString = function(){
		var o = this;
		return (o._s ? "-" : "") + (o._d.slice(0, o._f).join("") || "0") + (o._f != o._d.length ? "." + o._d.slice(o._f).join("") : "");
	};
	o._zeroes = function(n, l, t){
		var s = ["push", "unshift"][t || 0];
		for(++l; --l;  n[s](0));
		return n;
	};
	o.round = function(){
		if("_rounding" in this) return this;
		var $ = BigNumber, r = this.roundType, b = this._d, d, p, n, x;
		for(this._rounding = true; this._f > 1 && !b[0]; --this._f, b.shift());
		for(d = this._f, p = this.precision + d, n = b[p]; b.length > d && !b[b.length -1]; b.pop());
		x = (this._s ? "-" : "") + (p - d ? "0." + this._zeroes([], p - d - 1).join("") : "") + 1;
		if(b.length > p){
			n && (r == $.DOWN ? false : r == $.UP ? true : r == $.CEIL ? !this._s
			: r == $.FLOOR ? this._s : r == $.HALF_UP ? n >= 5 : r == $.HALF_DOWN ? n > 5
			: r == $.HALF_EVEN ? n >= 5 && b[p - 1] & 1 : false) && this.add(x);
			b.splice(p, b.length - p);
		}
		return delete this._rounding, this;
	};
}