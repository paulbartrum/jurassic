/// Copyright (c) 2012 Ecma International.  All rights reserved. 
/// Ecma International makes this code available under the terms and conditions set
/// forth on http://hg.ecmascript.org/tests/test262/raw-file/tip/LICENSE (the 
/// "Use Terms").   Any redistribution of this code must retain the above 
/// copyright and this notice and otherwise comply with the Use Terms.

/* Handles updating the page with information from the runner. */
function Presenter() {
    var altStyle = '',
        logger,
        date,
        version,
        table,
        backLink,

        globalSection = new Section(null, "0", "ECMA-262"),
        currentSection = globalSection,
        tests = {},
        totalTests = 0;

    var progressBar;
    TOCFILEPATH = "metadata/ecma-262-toc.xml";
  //**INTERFACE****************************************************************
  /* Updates progress with the given test, which should have its results in it as well. */
    this.addTestResult = function(test) {
        tests[test.id] = test;
        getSectionById(test.id).addTest(test);

        updateCounts();

        //TODO: eventually remove this guard.
        if(test.result === 'fail') {
            logResult(test);
        }
    }
    
    this.setVersion = function(v) {
        version = v;
        $(".targetTestSuiteVersion").text(v);
    }
  
  this.setDate = function(d) {
        date = d;
        $(".targetTestSuiteDate").text(d);
    }
  
    this.setTotalTests = function(tests) {
        totalTests = tests;
        $('#testsToRun').text(tests);
    }
  
  /* Write status to the activity bar. */
    this.updateStatus = function (str) {
       this.activityBar.text(str);
    }
  
  this.finished = function(elapsed) {
        $('.button-start').attr('src', 'images/start.png');
        $('.button-start').fadeOut('fast');
        
        progressBar.find(".text").html("Testing complete!"); 
        if (isSiteDebugMode()) {
            this.activityBar.text('Overall Execution Time: ' + elapsed + ' minutes');
        } else {
            this.activityBar.text('');
        }
    }
  
  this.started = function () {
        $('.button-start').attr('src', 'images/pause.png');
    }

    this.paused = function () {
        $('.button-start').attr('src', 'images/resume.png');
    }

    this.reset = function() {
        globalSection.reset();
        updateCounts();
        logger.empty();

        currentSection = globalSection;
        renderCurrentSection();
        $('.button-start').show();
    }
  
  
  /* Do some setup tasks. */
    this.setup = function() {
        backLink = $('#backlinkDiv');
        backLink.click(goBack);
        table = $('.results-data-table');
        
        logger = $("#tableLogger");
        progressBar = $('#progressbar');
        this.activityBar = $('#nextActivity');
        
        $('a.showSource', logger).live("click", openSourceWindow);
        $('a.showError', logger).live("click", openErrorWindow);
        $('#ancGenXMLReport').click(createXMLReportWindow);
    }
    
    /* Refresh display of the report */
    this.refresh = function() {
        renderCurrentSection();
    }
    
  //**IMPLEMENTATION DETAILS***************************************************

    /* Renders the current section into the report window. */
    function renderCurrentSection() {
        renderBreadcrumbs();
        if(globalSection.totalTests === 0) {
            $('#resultMessage').show();
        } else {
            $('#resultMessage').hide();
        }

        $('.totalCases').text(currentSection.totalTests);
        $('.passedCases').text(currentSection.totalPassed);
        $('.failedCases').text(currentSection.totalFailed);
        $('#failedToLoadCounterDetails').text(currentSection.totalFailedToLoad);
        table.empty();
        table.append(currentSection.toHTML());
        // Observe section selection and show source links
        $('a.section', table).click(sectionSelected);
        $('a.showSource', table).click(openSourceWindow);
    }

    /* Opens a window with a test's source code. */
    function openSourceWindow(e) {
        var test = tests[e.target.href.match(/#(.+)$/)[1]],
            popWnd = window.open("", "", "scrollbars=1, resizable=1"),
            innerHTML = '';

        innerHTML += '<b>Test </b>';
        innerHTML += '<b>' + test.id + '</b> <br /><br />\n';

        if (test.description) {
            innerHTML += '<b>Description</b>';
            innerHTML += '<pre>' +
                test.description.replace(/</g, '&lt;').replace(/>/g, '&gt;') +
                ' </pre>\n';
        }

        innerHTML += '<br /><br /><br /><b>Testcase</b>';
        innerHTML += '<pre>' + test.code + '</pre>\n';

        innerHTML += '<br /><b>Path</b>';
        innerHTML += '<pre>' + test.path + '</pre>';
        innerHTML += '<br /><a href="javascript:void(window.open(\'http://hg.ecmascript.org/tests/test262/file/tip/test/suite'
        innerHTML += test.path.replace("TestCases", "") + '\'));">' + 'Hg source' + '</a> (might be newer than the testcase source shown above)\n'
        
        popWnd.document.write(innerHTML);
    }

    /* Opens a window with a test's failure message. */
    function openErrorWindow(e) {
        var test = tests[e.target.href.match(/#(.+)$/)[1]],
            popWnd = window.open("", "", "scrollbars=1, resizable=1"),
            innerHTML = '';

        var bugDetails = "";
        bugDetails    += "DESCRIPTION\n*Please insert your description here!*\n\n";
        bugDetails    += "------------------\n";
        bugDetails    += "TEST:            " + test.path + "\n";
        bugDetails    += "SOURCE:          http://hg.ecmascript.org/tests/test262/file/tip/test/suite" + test.path.replace("TestCases", "") + "\n";
        bugDetails    += "TEST SUITE DATE: " + date + "\n";
        bugDetails    += "PLATFORM:        " + navigator.userAgent + "\n";
        bugDetails    += "ERROR:           " + test.error + "\n\n";

        
        var bugTemplate = 'https://bugs.ecmascript.org/enter_bug.cgi?product=Test262&amp;bug_severity=normal&amp;component=Tests&amp;short_desc=';
        bugTemplate += encodeURIComponent('Invalid test? ' + test.id) + "&amp;comment=";
        bugTemplate += encodeURIComponent(bugDetails);
        
        innerHTML += '<b>Test </b>';
        innerHTML += '<b>' + test.id + '</b> <br /><br />\n';

        innerHTML += '<b>Failure</b>';
        innerHTML += '<pre>' + test.error + '</pre>\n';
        
        innerHTML += '<br /><br /><b>Testcase</b>';
        innerHTML += '<pre>' + test.code + '</pre>\n';
        
        innerHTML += '<br /><br /><b>Broken test?</b>';
        innerHTML += '<p>If you have reason to believe the JavaScript engine being tested<br />\n';
        innerHTML += 'is actually OK and there\'s instead something wrong with the test<br />\n';
        innerHTML += 'itself, please <a href="' + bugTemplate + '" onclick="window.moveTo(0,0);window.resizeTo(screen.width, screen.height);">file a bug.</a></p>\n'
        
        popWnd.document.write(innerHTML);
    }

    /* Returns the section object for the specified section id
     * (eg. "7.1" or "15.4.4.12").
     */
    function getSectionById(id) {
        if(id == 0)
            return globalSection;

        var match = id.match(/\d+/g);
        var section = globalSection;

        for(var i = 0; i < match.length; i++) {
            if(typeof section.subsections[match[i]] !== "undefined") {
                section = section.subsections[match[i]];
            } else {
                break;
            }
        }
        return section;
    }

    /* Update the page with current status */
    function updateCounts() {
        $('#Pass').text(globalSection.totalPassed);
        $('#Fail').text(globalSection.totalFailed);
        $('#totalCounter').text(globalSection.totalTests);
        $('#failedToLoadCounter1').text(globalSection.totalFailedToLoad);
        $('#failedToLoadCounter').text(globalSection.totalFailedToLoad);
        progressBar.reportprogress(globalSection.totalTests, totalTests);
    }

    /* Append a result to the run page's result log. */
    function logResult(test) {
        var appendStr = "";
        altStyle = (altStyle !== ' ') ? ' ' : 'alternate';
        if (test.result==="fail") {
            appendStr += '<tbody>';
            appendStr += '<tr class=\"' + altStyle + '\">';
            
            appendStr += '<td width=\"20%\">';
            appendStr += "<a class='showSource' href='#" + test.id + "'>";
            appendStr += test.id + "</a>";
            appendStr += '</td>';
            
            appendStr += '<td>' + test.description + '</td>';
            
            appendStr += '<td align="right">';
            appendStr += '<span class=\"Fail\">' + "<a class='showError' href='#" + test.id + "'>";
            appendStr += 'Fail</a></span></td></tr></tbody>';
        }
        
        else if (test.result==="pass") {
           if  (! isSiteDebugMode()) { return;}
            appendStr += '<tbody><tr class=\"' + altStyle + '\"><td width=\"20%\">';
            appendStr += "<a class='showSource' href='#" + test.id + "'>";
            appendStr += test.id + "</a>" + '</td><td>' + test.description;
            appendStr += '</td><td align="right"><span class=\"Fail\">';
            appendStr += 'Pass</span></td></tr></tbody>';
        }
        else {
            throw "Result for '" + test.id + "' must either be 'pass' or 'fail', not '" + test.result + "'!";
        }
    
            
        logger.append(appendStr);
        logger.parent().attr("scrollTop", logger.parent().attr("scrollHeight"));
    }
    
    
    
    //*************************************************************************
    /* Go back to the previous section */
    function goBack(e) {
        e.preventDefault();

        if(currentSection === globalSection)
            return;

        currentSection = currentSection.parentSection;

        // Since users click directly on sub-chapters of the main chapters, don't go back to main
        // chapters.
        if(currentSection.parentSection === globalSection)
            currentSection = globalSection;

        renderCurrentSection();
    }
    
    /* Load the table of contents xml to populate the sections. */
    function loadSections() {
        var sectionsLoader = new XMLHttpRequest();
        sectionsLoader.open("GET", TOCFILEPATH, false);
        sectionsLoader.send();
        var xmlDoc = sectionsLoader.responseXML;
        var nodes = xmlDoc.documentElement.childNodes;

        addSectionsFromXML(nodes, globalSection);
    }


    /* Recursively parses the TOC xml, producing nested sections. */
    function addSectionsFromXML(nodes, parentSection){
        var subsection;

        for (var i = 0; i < nodes.length; i++) {
            if (nodes[i].nodeName === "sec") {
                subsection = new Section(parentSection, nodes[i].getAttribute('id'), nodes[i].getAttribute('name'));
                parentSection.subsections[subsection.id.match(/\d+$/)] = subsection;
                addSectionsFromXML(nodes[i].childNodes, subsection);
            }
        }
    }
    
    /* Renders the breadcrumbs for report navigation. */
    function renderBreadcrumbs() {
        var container = $('div.crumbContainer div.crumbs');
        var sectionChain = [];

        var current = currentSection;

        // Walk backwards until we reach the global section.
        while(current !== globalSection && current.parentSection !== globalSection) {
            sectionChain.push(current);
            current = current.parentSection;
        }

        // Reverse the array since we want to print earlier sections first.
        sectionChain.reverse();

        // Empty any existing breadcrumbs.
        container.empty();

        // Static first link to go back to the root.
        var link = $("<a href='#0' class='setBlack'>Test Sections &gt; </a>");
        link.bind('click', {sectionId: 0}, sectionSelected)
        container.append(link);

        for(var i = 0; i < sectionChain.length;i++) {
            link = $("<a href='#" + sectionChain[i].id + "' class='setBlack'>" + sectionChain[i].id + ": " + sectionChain[i].name + " &gt; </a>");
            link.bind('click', sectionSelected)
            container.append(link);
        }

        // If we can go back, show the back link.
        if(sectionChain.length > 0) {
            backLink.show();
        } else {
            backLink.hide();
        }
    };
    
    /* Pops up a window with an xml dump of the results of a test. */
    function createXMLReportWindow() {
        var reportWindow; //window that will output the xml data
        var xmlData;      //array instead of string concatenation
        var dateNow;
        var xml;  // stop condition of for loop stored in a local variable to improve performance

        dateNow = new Date();

        xml = '<testRun>\r\n' +
              '<userAgent>' + window.navigator.userAgent + '</userAgent>\r\n' +
			  '<Date>' + dateNow.toDateString() + '</Date>\r\n' +
			  '<targetTestSuiteName>ECMAScript Test262 Site</targetTestSuiteName>\r\n' +
			  '<targetTestSuiteVersion>' + version + '</targetTestSuiteVersion>\r\n' +
			  '<targetTestSuiteDate>' + date + '</targetTestSuiteDate>\r\n' +
			  ' <Tests>\r\n\r\n';

        reportWindow = window.open();
        reportWindow.document.writeln("<title>ECMAScript Test262 XML</title>");
        reportWindow.document.write("<textarea id='results' style='width: 100%; height: 800px;'>");
        reportWindow.document.write(xml);
        reportWindow.document.write(globalSection.toXML());
        reportWindow.document.write('</Tests>\r\n</testRun>\r\n</textarea>\r\n');
        reportWindow.document.close();
    }

    /* Callback for when the user clicks on a section in the report table. */
    function sectionSelected(e) {
        e.preventDefault();
        currentSection = getSectionById(e.target.href.match(/#(.+)$/)[1]);
        renderCurrentSection();
        table.attr("scrollTop", 0);
    };
    
    //*************************************************************************
    // Load the sections.
    loadSections();
}

var presenter = new Presenter();
