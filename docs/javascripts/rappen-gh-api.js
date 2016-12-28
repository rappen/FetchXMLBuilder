var RAPPEN_GH_PAGE_SIZE = 100;

LoadPeople = function (apimethod, element, cols) {
    $.ajax({
        url: 'https://api.github.com/repos/' + GH_USER + '/' + GH_REPO + '/' + apimethod,
        success: function (data) {
            var gazers = "";
            if (data) {
                data.sort(sort_by('login', false, function (a) { return a.toUpperCase() }));
                $(data).each(function (index) {
                    gazers += "<a href='" + this.html_url + "' id='" + apimethod + "_" + this.login + "' target='_blank'><img src='" + this.avatar_url + "' height='50' width='50'/></a>";
                    if ((index + 1) % cols == 0) {
                        gazers += "<br/>";
                    }
                    GetUserInfo(this, apimethod);
                });
            }
            if (gazers) {
                gazers = //"--starred by--<br/>" +
                    gazers + "<br/><br/>";
            }
            $("#" + element).html(gazers);
        }
    });
};

GetUserInfo = function (user, target) {
    $.ajax({
        url: 'https://api.github.com/users/' + user.login,
        success: function (data) {
            var info = "";
            if (data) {
                if (data.name) {
                    info = data.name + " (" + user.login + ")";
                } else {
                    info = user.login;
                }
                if (data.company) {
                    info += "\n" + data.company;
                }
                if (data.location) {
                    info += "\n" + data.location;
                }
                if (data.hireable) {
                    info += "\n\nFOR HIRE!";
                }
            }
            var element = "#" + target + "_" + user.login;
            $(element).attr('title', info);
        }
    });
};

UpdateDownloads = function (version, published, currentcount, releaselink) {
    $("#" + version).text("Loading...");
    $("#" + published).text("Loading...");
    $("#" + currentcount).text("Loading...");
    $.ajax({
        url: 'https://api.github.com/repos/' + GH_USER + '/' + GH_REPO + '/releases/latest',
        success: function (data) {
            if (data && data.assets && data.assets.length > 0) {
                var count = data.assets[0].download_count;
                var tag = data.tag_name;
                if (GH_REPO == "FetchXMLBuilder" && tag == "1.2015.1.10") {
                    // Add codeplex count
                    count += 339;
                    // Del test count
                    count -= 12;
                }
                var date = new Date(data.published_at);
                $("#" + version).text(data.tag_name);
                $("#" + published).text(date.toFormattedString('yyyy-mm-dd'));
                $("#" + currentcount).text(count);
                $("#latest-download span").text("Download " + tag);
                $("#latest-download").attr('href', data.assets[0].browser_download_url);
                $("#" + releaselink).attr('href', data.html_url);
                //var notes = data.body;
                //var converter = new Showdown.converter();
                //var htmlnotes = converter.makeHtml(notes);
                //$("#latest-notes").text(htmlnotes);
            } else {
                $("#" + version).text("");
                $("#" + published).text("");
                $("#" + currentcount).text("");
                $("#latest-download span").text("No download available");
                $("#latest-download").attr('href', "#");
            }
        },
        error: function (xhr, options, error) {
            $("#" + published).text("");
            $("#" + currentcount).text("");
            if (xhr && xhr.status && xhr.status == 403) {
                $("#" + version).text("");
                //if (xhr.responseText) {
                //    var response = JSON.parse(xhr.responseText);
                //    if (response.message) {
                //        $("#latest-version").text(response.message);
                //    }
                //}
                $("#latest-download span").text("You really want it, right!?");
                $("#latest-download").attr('href', 'https://github.com/' + GH_USER + '/' + GH_REPO + '/releases');
            }
            else {
                $("#" + version).text(error);
                $("#latest-download span").text("");
                $("#latest-download").attr('href', "#");
            }
        }
    });
};

GetLatestDownloadLink = function () {
    var url = "";
    $.ajax({
        async: false,
        url: 'https://api.github.com/repos/' + GH_USER + '/' + GH_REPO + '/releases/latest',
        success: function (data) {
            if (data && data.assets && data.assets.length > 0) {
                url = data.assets[0].browser_download_url;
            }
        },
        error: function (xhr, options, error) {
            return "/";
        }
    });

    return url;
};

GetLatestVersion = function () {
    var version = "";
    $.ajax({
        async: false,
        url: 'https://api.github.com/repos/' + GH_USER + '/' + GH_REPO + '/releases/latest',
        success: function (data) {
            if (data) {
                version = data.tag_name;
            }
        },
        error: function (xhr, options, error) {
        }
    });
    return version;
};

UpdateTotalDownloads = function (totalcount) {
    $("#" + totalcount).text("");
    $.ajax({
        url: 'https://api.github.com/repos/' + GH_USER + '/' + GH_REPO + '/releases',
        success: function (data) {
            if (data && data.length > 0) {
                var count = 0;
                $(data).each(function (index) {
                    if (this.assets.length > 0) {
                        count += this.assets[0].download_count;
                    }
                });
                if (GH_REPO == "FetchXMLBuilder") {
                    // Add codeplex count
                    count += 1010;   // Updated 2015-04-15
                }
                $("#" + totalcount).text(" (" + count + ")");
            }
        }
    });
};

UpdateHistoricDownloads = function (histcount, callback, includedate) {
    $("#" + histcount).text("");
    $.ajax({
        url: 'https://api.github.com/repos/' + GH_USER + '/' + GH_REPO + '/releases',
        success: function (data) {
            if (data && data.length > 0) {
                var counttext = "<u>Version&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Count&nbsp;&nbsp;Released</u><br />";
                $(data).each(function (index) {
                    var tag = this.tag_name.padRight(13);
                    var date = new Date(this.published_at);
                    if (includedate) {
                        date = "&nbsp;&nbsp;" + date.toFormattedString('yyyy-mm-dd');
                    }
                    else {
                        date = "";
                    }
                    var count = 0;
                    $(this.assets).each(function (index2) {
                        count += this.download_count;
                    });
                    counttext += tag + " <strong>" + count.padLeft(5, '&nbsp;') + "</strong>" + date + "<br/>";
                });
                if (GH_REPO == "FetchXMLBuilder") {
                    counttext += GetCodePlexDownloads(includedate);
                }
                counttext += "";
                $("#" + histcount).html(counttext);
            }
            if (callback) {
                callback();
            }
        }
    });
};

GetCodePlexDownloads = function (includedate) {       // Updated 2015-04-15
    var template = "{tag}&nbsp;&nbsp;<strong>{count}</strong>&nbsp;&nbsp;{date}<br/>";
    var cp = "<i>&nbsp;-at codeplex-<br/>";
    var ver = "9 versions&nbsp;&nbsp;&nbsp;";
    cp += template.replace("{tag}", ver).replace("{count}", (1010).padLeft(4, '&nbsp;')).replace("{date}", includedate ? "2014-11-21" : "");
    //cp += template.replace("{tag}", "1.2015.1.10").replace("{count}", (389).padLeft(3, '&nbsp;')).replace("{date}", includedate ? "(2015-01-26)" : "");
    //cp += template.replace("{tag}", "1.2015.1.9 ").replace("{count}", (46).padLeft(3, '&nbsp;')).replace("{date}", includedate ? "(2015-01-20)" : "");
    //cp += template.replace("{tag}", "1.2014.12.8").replace("{count}", (128).padLeft(3, '&nbsp;')).replace("{date}", includedate ? "(2014-12-30)" : "");
    //cp += template.replace("{tag}", "1.2014.12.7").replace("{count}", (47).padLeft(3, '&nbsp;')).replace("{date}", includedate ? "(2014-12-18)" : "");
    //cp += template.replace("{tag}", "1.2014.12.6").replace("{count}", (58).padLeft(3, '&nbsp;')).replace("{date}", includedate ? "(2014-12-05)" : "");
    //cp += template.replace("{tag}", "1.2014.12.5").replace("{count}", (76).padLeft(3, '&nbsp;')).replace("{date}", includedate ? "(2014-12-01)" : "");
    //cp += template.replace("{tag}", "1.2014.11.4").replace("{count}", (45).padLeft(3, '&nbsp;')).replace("{date}", includedate ? "(2014-11-25)" : "");
    //cp += template.replace("{tag}", "1.2014.11.3").replace("{count}", (23).padLeft(3, '&nbsp;')).replace("{date}", includedate ? "(2014-11-24)" : "");
    //cp += template.replace("{tag}", "1.2014.11.2").replace("{count}", (56).padLeft(3, '&nbsp;')).replace("{date}", includedate ? "(2014-11-21)" : "");
    cp += "</i>";
    return cp;
};

UpdateReleaseNotes = function (releasenotes, callback) {
    $("#" + releasenotes).text("Loading...");
    $.ajax({
        url: 'https://api.github.com/repos/' + GH_USER + '/' + GH_REPO + '/releases/latest',
        success: function (data) {
            if (data && data.assets && data.assets.length > 0) {
                var notes = data.body;
                var converter = new Showdown.converter();
                var htmlnotes = converter.makeHtml(notes);
                // Correction for github flavor of markdown, issue references
                htmlnotes = htmlnotes.replace(/<h1>/g, '#').replace('</h1>', '');
                htmlnotes = htmlnotes.replace(/<p>/g, '<br/><br/><p>');
                $("#" + releasenotes).html(htmlnotes);
            } else {
                $("#" + releasenotes).text("");
            }
            if (callback) {
                callback();
            }
        },
        error: function (xhr, options, error) {
            $("#" + releasenotes).text("");
            if (xhr && xhr.status && xhr.status == 403) {
                $("#" + releasenotes).text("You really want it, right!?");
            }
            else {
                $("#" + releasenotes).text(error);
            }
        }
    });
};

LoadIssues = function (open, total) {
    $("#" + open).text("?");
    $("#" + total).text("?");
    $.ajax({
        url: 'https://api.github.com/repos/' + GH_USER + '/' + GH_REPO + '/issues?state=open&per_page=' + RAPPEN_GH_PAGE_SIZE,
        success: function (data) {
            var count = 0;
            if (data) {
                if (data.length >= RAPPEN_GH_PAGE_SIZE) {
                    count = RAPPEN_GH_PAGE_SIZE + "+";
                } else {
                    count = data.length;
                }
            }
            $("#" + open).text(count);
        }
    });
    $.ajax({
        url: 'https://api.github.com/repos/' + GH_USER + '/' + GH_REPO + '/issues?state=all&per_page=' + RAPPEN_GH_PAGE_SIZE,
        success: function (data) {
            var count = 0;
            if (data) {
                if (data.length >= RAPPEN_GH_PAGE_SIZE) {
                    count = RAPPEN_GH_PAGE_SIZE + "+";
                } else {
                    count = data.length;
                }
            }
            $("#" + total).text(count);
        }
    });
};

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
