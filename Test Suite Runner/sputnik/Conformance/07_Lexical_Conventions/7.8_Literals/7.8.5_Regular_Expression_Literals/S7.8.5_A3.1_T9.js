// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/**
 * @name: S7.8.5_A3.1_T9;
 * @section: 7.8.5, 15.10.4.1;
 * @assertion: RegularExpressionFlags :: IdentifierPart;
 * @description: IdentifierPart :: \u006D (m);
*/

//CHECK#1
var regexp = /(?:)/\u006D; 
if (regexp.multiline !== true) {
  $ERROR('#1: var regexp = /(?:)/\\u006D; regexp.multiline === true. Actual: ' + (regexp.multiline));
}                             
