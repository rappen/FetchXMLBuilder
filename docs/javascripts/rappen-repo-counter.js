var RAPPEN_NUGET_API_QUERY = 'https://api-v3search-0.nuget.org/query?q=id:{package}';
var RAPPEN_NUGET_API_DETAIL = 'http://api.nuget.org/v3/registration1/{package}/index.json';
var RAPPEN_NUGET_API_v2v3 = 'https://api-v2v3search-0.nuget.org/query?q=id:{package}';

NuGetGetApiUrl = function (package) {
    var apiurl = RAPPEN_NUGET_API_v2v3.replace("{package}", NUGET_PACKAGE).toLowerCase();
    return apiurl;
};

NugetGetDetails = function (packageName, successCallback, errorCallback) {
    $.ajax({
        url: NuGetGetApiUrl(packageName),
        crossDomain: true,
        success: function (data) {
            if (data && data.data && data.data.length > 0) {
                var result = data.data[0];
                result.host = "NuGet";
                var version = result.version;
                if (result.versions && result.versions.length > 0) {
                    $(result.versions).each(function (index) {
                        this.host = "NuGet";
                        if (this.version == version) {
                            result.latestVersion = this;
                        }
                        var id = this['@id'];
                        if (id) {
                            var date = null;
                            $.ajax({
                                url: id,
                                async: false,
                                success: function (data) {
                                    date = data.published;
                                },
                                error: function (x, o, e) {

                                }
                            });
                            this.published = date;
                        }
                    });
                }
            }
            successCallback(result);
        },
        error: function (xhr, options, error) {
            console.dir(xhr);
            console.log("XHR: " + xhr.toString());
            console.log("OPT: " + options);
            console.log("ERR: " + error);
            errorCallback(xhr, options, error);
        }
    });
};

NugetGetJson = function (url, successCallback, errorCallback) {
    $.ajax({
        url: url,
        crossDomain: true,
        dataType: 'jsonp',
        success: function (data) {
            successCallback(data);
        },
        error: function (xhr, options, error) {
            console.dir(xhr);
            console.log("XHR: " + xhr.toString());
            console.log("OPT: " + options);
            console.log("ERR: " + error);
            errorCallback(xhr, options, error);
        }
    });
};

GitHubGetDetails = function (user, repo, successCallback, errorCallback) {
    $.ajax({
        url: 'https://api.github.com/repos/' + user + '/' + repo + '/releases',
        crossDomain: true,
        dataType: 'jsonp',
        success: function (data) {
            var result = {};
            result.host = "GitHub";
            result.totalDownloads = 0;
            result.versions = [];
            if (data.data && data.data.length > 0) {
                $(data.data).each(function (index) {
                    var version = {};
                    version.version = this.tag_name;
                    version.published = new Date(this.published_at);
                    var count = 0;
                    $(this.assets).each(function (index2) {
                        count += this.download_count;
                    });
                    version.downloads = count;
                    version.host = "GitHub";
                    version.url = this.html_url;
                    result.totalDownloads += count;
                    result.versions.push(version);
                    if (!result.latestVersion) {
                        result.latestVersion = version;
                        result.projectUrl = result.latestVersion.html_url;
                    }
                });
            }
            successCallback(result);
        },
        error: function (xhr, options, error) {
            console.dir(xhr);
            console.log("XHR: " + xhr.toString());
            console.log("OPT: " + options);
            console.log("ERR: " + error);
            errorCallback(xhr, options, error);
        }
    });
};

CombineDetails = function (info1, info2, mergeVersions) {
    var result = info1;
    result.totalDownloads += info2.totalDownloads;
    var versions = info1.versions.concat(info2.versions);
    versions.sort(dynamicSortMultiple("-version", "-host"));
    var lastVersion = "";
    result.versions = [];
    $(versions).each(function (i) {
        if (this.version != lastVersion || !!!mergeVersions) {
            result.versions.push(this);
        }
        else {
            result.versions[result.versions.length - 1].downloads += this.downloads;
        }
        lastVersion = this.version;
    });
    result.latestVersion = null;
    $(result.versions).each(function (i) {
        if (!result.latestVersion || this.version > result.latestVersion.version) {
            result.latestVersion = this;
        }
    });
    return result;
}

// Found here: http://stackoverflow.com/questions/1129216/sort-array-of-objects-by-string-property-value-in-javascript
var dynamicSort = function (property) {
    var sortOrder = 1;
    if (property[0] === "-") {
        sortOrder = -1;
        property = property.substr(1);
    }
    return function (a, b) {
        var result = (a[property] < b[property]) ? -1 : (a[property] > b[property]) ? 1 : 0;
        return result * sortOrder;
    }
}

var dynamicSortMultiple = function () {
    /*
     * save the arguments object as it will be overwritten
     * note that arguments object is an array-like object
     * consisting of the names of the properties to sort by
     */
    var props = arguments;
    return function (obj1, obj2) {
        var i = 0, result = 0, numberOfProperties = props.length;
        /* try getting a different result from 0 (equal)
         * as long as we have extra properties to compare
         */
        while (result === 0 && i < numberOfProperties) {
            result = dynamicSort(props[i])(obj1, obj2);
            i++;
        }
        return result;
    }
}

var sort_by = function (field, reverse, primer) {

    var key = primer ?
        function (x) { return primer(x[field]) } :
        function (x) { return x[field] };

    reverse = !reverse ? 1 : -1;

    return function (a, b) {
        return a = key(a), b = key(b), reverse * ((a > b) - (b > a));
    }
};

Date.prototype.toFormattedString = function (format) {
    /// <summary>
    /// Formats date string dd (date), mm (month), yyyy (year), MM (min), hh (hour), ss (seconds), ms (millisec), APM (AM/PM)
    /// </summary>
    /// <param name="format"></param>
    /// <returns type=""></returns>
    var d = this;
    var f = "";
    f = f + format.replace(/dd|mm|yyyy|MM|hh|ss|ms|APM|\s|\/|\-|,|\./ig, function match() {
        switch (arguments[0]) {
            case "dd":
                var dd = d.getDate();
                return (dd < 10) ? "0" + dd : dd;
            case "mm":
                var mm = d.getMonth() + 1;
                return (mm < 10) ? "0" + mm : mm;
            case "yyyy": return d.getFullYear();
            case "hh":
                var hh = d.getHours();
                return (hh < 10) ? "0" + hh : hh;
            case "MM":
                var MM = d.getMinutes();
                return (MM < 10) ? "0" + MM : MM;
            case "ss":
                var ss = d.getSeconds();
                return (ss < 10) ? "0" + ss : ss;
            case "ms": return d.getMilliseconds();
            case "APM":
                var apm = d.getHours();
                return (apm < 12) ? "AM" : "PM";
            default: return arguments[0];
        }
    } // end match function
    ); // end format.replace
    return f;
};

Number.prototype.padLeft = function (n, str) {
    return Array(n - String(this).length + 1).join(str || '0') + this;
};

String.prototype.padRight = function (n, str) {
    return this + Array(n - this.length + 1).join(str || '&nbsp;');
};
