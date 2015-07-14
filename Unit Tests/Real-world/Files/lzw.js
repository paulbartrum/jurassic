//LZW Compression/Decompression for Strings
var LZW = {
    "compress": function (uncompressed) {
        // Build the dictionary.
        var dictSize = 256;
        var dictionary = {};
        for (var i = 0; i < 256; i++) {
            dictionary[String.fromCharCode(i)] = i;
        }

        var w = "";
        var result = [];
        for (var i = 0; i < uncompressed.length; i++) {
            var c = uncompressed.charAt(i);
            var wc = w + c;
            if (dictionary[wc])
                w = wc;
            else {
                result.push(dictionary[w]);
                // Add wc to the dictionary.
                dictionary[wc] = dictSize++;
                w = "" + c;
            }
        }

        // Output the code for w.
        if (w != "")
            result.push(dictionary[w]);
        return result;
    },


    "decompress": function (compressed) {
        // Build the dictionary.
        var dictSize = 256;
        var dictionary = [];
        for (var i = 0; i < 256; i++) {
            dictionary[i] = String.fromCharCode(i);
        }

        var w = String.fromCharCode(compressed[0]);
        var result = w;
        for (var i = 1; i < compressed.length; i++) {
            var entry = "";
            var k = compressed[i];
            if (dictionary[k])
                entry = dictionary[k];
            else if (k == dictSize)
                entry = w + w.charAt(0);
            else
                return null;

            result += entry;

            // Add w+entry[0] to the dictionary.
            dictionary[dictSize++] = w + entry.charAt(0);

            w = entry;
        }
        return result;
    }
}