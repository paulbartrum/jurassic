using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jurassic;
using Jurassic.Compiler;

namespace UnitTests
{
    /// <summary>
    /// Real-world tests.
    /// </summary>
    [TestClass]
    public class RealWorldTests
    {
        [TestMethod]
        public void Showdown()
        {
            // See http://attacklab.net/showdown/
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\showdown.js");
            engine.Execute("var converter = new Showdown.converter()");
            engine.SetGlobalValue("text", TestUtils.NormalizeText(@"
                Showdown Demo
                -------------

                You can try out Showdown on this page:

                  - Type some [Markdown] text on the left side.
                  - See the corresponding HTML on the right.

                For a Markdown cheat-sheet, switch the right-hand window from *Preview* to *Syntax Guide*."));
            Assert.AreEqual(TestUtils.NormalizeText(@"
                <h2>Showdown Demo</h2>

                <p>You can try out Showdown on this page:</p>

                <ul>
                <li>Type some [Markdown] text on the left side.</li>
                <li>See the corresponding HTML on the right.</li>
                </ul>

                <p>For a Markdown cheat-sheet, switch the right-hand window from <em>Preview</em> to <em>Syntax Guide</em>.</p>"),
                TestUtils.NormalizeText(engine.Evaluate<string>("converter.makeHtml(text)")));
        }

        [TestMethod]
        public void ColorConversion()
        {
            // http://www.webtoolkit.info/javascript-color-conversion.html
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\color-conversion.js");
            engine.Execute("var result = ColorConverter.toRGB(new HSV(10, 20, 30))");
            Assert.AreEqual(77, engine.Evaluate("result.r"));
            Assert.AreEqual(64, engine.Evaluate("result.g"));
            Assert.AreEqual(61, engine.Evaluate("result.b"));
        }

        [TestMethod]
        public void sprintf()
        {
            // From http://phpjs.org/functions/sprintf
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\sprintf.js");
            Assert.AreEqual("123.10", engine.Evaluate("sprintf('%01.2f', 123.1)"));
            Assert.AreEqual("[    monkey]", engine.Evaluate("sprintf('[%10s]', 'monkey')"));
            Assert.AreEqual("[####monkey]", engine.Evaluate("sprintf(\"[%'#10s]\", 'monkey')"));
        }

        [TestMethod]
        public void MD5()
        {
            // From http://www.webtoolkit.info/javascript-md5.html
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\md5.js");
            Assert.AreEqual("ae2b1fca515949e5d54fb22b8ed95575", engine.Evaluate(@"MD5('testing')"));
            Assert.AreEqual("023c0c18f0c6e89076d668146fcb81c2", engine.Evaluate(@"MD5('Mary had a little lamb, it\'s fleece was white as snow!')"));
            Assert.AreEqual("cbbcd86416057ca304141fc9b3b418d5", engine.Evaluate(@"MD5('\u2020')"));
        }

        [TestMethod]
        public void SHA1()
        {
            // From http://www.webtoolkit.info/javascript-sha1.html
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\sha1.js");
            Assert.AreEqual("dc724af18fbdd4e59189f5fe768a5f8311527050", engine.Evaluate(@"SHA1('testing')"));
            Assert.AreEqual("5fb03a9e2c9d14894b51a2bd0e521177e083af3f", engine.Evaluate(@"SHA1('Mary had a little lamb, it\'s fleece was white as snow!')"));
            Assert.AreEqual("5b7c3f4be781869083966e4b5eac6bd2900d9340", engine.Evaluate(@"SHA1('\u2020')"));
        }

        [TestMethod]
        public void SHA256()
        {
            // From http://www.webtoolkit.info/javascript-sha256.html
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\sha256.js");
            Assert.AreEqual("cf80cd8aed482d5d1527d7dc72fceff84e6326592848447d2dc0b0e87dfc9a90", engine.Evaluate(@"SHA256('testing')"));
            Assert.AreEqual("68aa952fc1ee38fd07c7d58e693b5e6bebaf183f1b47a1fc9f41cd42bbb427d2", engine.Evaluate(@"SHA256('Mary had a little lamb, it\'s fleece was white as snow!')"));
            Assert.AreEqual("8efeb7661b801b1f4f1286b262b89ec8550bfbb4b438fe9fb31be551e747a547", engine.Evaluate(@"SHA256('\u2020')"));
        }

        [TestMethod]
        public void LZW()
        {
            // From http://rosettacode.org/wiki/LZW_compression#JavaScript
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\lzw.js");
            Assert.AreEqual("109,97,114,121,32,104,97,100,32,97,32,108,105,116,116,108,101,266,97,109,98",
                engine.Evaluate(@"LZW.compress('mary had a little lamb').toString()"));
            Assert.AreEqual("mary had a little lamb",
                engine.Evaluate(@"LZW.decompress([109,97,114,121,32,104,97,100,32,97,32,108,105,116,116,108,101,266,97,109,98]).toString()"));
            Assert.AreEqual("122,256,257,258,259,260,257",
                engine.Evaluate(@"LZW.compress('zzzzzzzzzzzzzzzzzzzzzzzz').toString()"));
            Assert.AreEqual("zzzzzzzzzzzzzzzzzzzzzzzz",
                engine.Evaluate(@"LZW.decompress([122,256,257,258,259,260,257]).toString()"));
        }

        [TestMethod]
        [Ignore]
        public void RSAEncrypt()
        {
            // From http://xenon.stanford.edu/~tjw/jsbn/
            var engine = new ScriptEngine();
            engine.EnableDebugging = true;
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\rsa.js");
            engine.Execute(@"
                var rsa = new RSAKey();
                rsa.setPublic('a5261939975948bb7a58dffe5ff54e65f0498f9175f5a09288810b8975871e99\n' +
                    'af3b5dd94057b0fc07535f5f97444504fa35169d461d0d30cf0192e307727c06\n' + 
                    '5168c788771c561a9400fb49175e9e6aa4e23fe11af69e9412dd23b0cb6684c4\n' +
                    'c2429bce139e848ab26d0829073351f4acd36074eafd036a5eb83359d2a698d3', '10001');
                var encrypted = rsa.encrypt('text to encrypt');
                rsa.setPrivateEx('a5261939975948bb7a58dffe5ff54e65f0498f9175f5a09288810b8975871e99\n' +
                    'af3b5dd94057b0fc07535f5f97444504fa35169d461d0d30cf0192e307727c06\n' + 
                    '5168c788771c561a9400fb49175e9e6aa4e23fe11af69e9412dd23b0cb6684c4\n' +
                    'c2429bce139e848ab26d0829073351f4acd36074eafd036a5eb83359d2a698d3',
                    '10001',
                    '8e9912f6d3645894e8d38cb58c0db81ff516cf4c7e5a14c7f1eddb1459d2cded\n4d8d293fc97aee6aefb861859c8b6a3d1dfe710463e1f9ddc72048c09751971c\n4a580aa51eb523357a3cc48d31cfad1d4a165066ed92d4748fb6571211da5cb1\n4bc11b6e2df7c1a559e6d5ac1cd5c94703a22891464fba23d0d965086277a161',
                    'd090ce58a92c75233a6486cb0a9209bf3583b64f540c76f5294bb97d285eed33\naec220bde14b2417951178ac152ceab6da7090905b478195498b352048f15e7d',
                    'cab575dc652bb66df15a0359609d51d1db184750c00c6698b90ef3465c996551\n03edbf0d54c56aec0ce3c4d22592338092a126a0cc49f65a4a30d222b411e58f',
                    '1a24bca8e273df2f0e47c199bbf678604e7df7215480c77c8db39f49b000ce2c\nf7500038acfff5433b7d582a01f1826e6f4d42e1c57f5e1fef7b12aabc59fd25',
                    '3d06982efbbe47339e1f6d36b1216b8a741d410b0c662f54f7118b27b9a4ec9d\n914337eb39841d8666f3034408cf94f5b62f11c402fc994fe15a05493150d9fd',
                    '3a3e731acd8960b7ff9eb81a7ff93bd1cfa74cbd56987db58b4594fb09c09084\ndb1734c8143f98b602b981aaa9243ca28deb69b5b280ee8dcee0fd2625e53250');");
            Assert.AreEqual("", engine.Evaluate("rsa.decrypt(encrypted)"));
        }

        [TestMethod]
        public void Levenshtein()
        {
            // From http://phpjs.org/functions/levenshtein
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\levenshtein.js");
            Assert.AreEqual(3, engine.Evaluate(@"levenshtein('Kevin van Zonneveld', 'Kevin van Sommeveld')"));
            Assert.AreEqual(5, engine.Evaluate(@"levenshtein('Phoney Malony', 'Fonee Malowney')"));
            Assert.AreEqual(7, engine.Evaluate(@"levenshtein('Phoney Malony', 'Bogey Gilooni')"));
            Assert.AreEqual(14, engine.Evaluate(@"levenshtein('Phoney Malony', 'Major Carrothead')"));
        }

        [TestMethod]
        public void CoffeeScript()
        {
            // From http://jashkenas.github.com/coffee-script/
            var engine = new ScriptEngine();
            engine.ExecuteFile(@"..\..\..\Unit Tests\Real-world\Files\coffee-script.js");
            
            engine.SetGlobalValue("script", @"
                # Assignment:
                number   = 42
                opposite = true

                # Conditions:
                number = -42 if opposite

                # Functions:
                square = (x) -> x * x

                # Arrays:
                list = [1, 2, 3, 4, 5]

                # Objects:
                math =
                  root:   Math.sqrt
                  square: square
                  cube:   (x) -> x * square x

                # Splats:
                race = (winner, runners...) ->
                  print winner, runners

                # Existence:
                alert ""I knew it!"" if elvis?

                # Array comprehensions:
                cubes = (math.cube num for num in list)");
            Assert.AreEqual(TestUtils.NormalizeText(@"
                (function() {
                  var cubes, list, math, num, number, opposite, race, square;
                  var __slice = Array.prototype.slice;
                  number = 42;
                  opposite = true;
                  if (opposite) {
                    number = -42;
                  }
                  square = function(x) {
                    return x * x;
                  };
                  list = [1, 2, 3, 4, 5];
                  math = {
                    root: Math.sqrt,
                    square: square,
                    cube: function(x) {
                      return x * square(x);
                    }
                  };
                  race = function() {
                    var runners, winner;
                    winner = arguments[0], runners = 2 <= arguments.length ? __slice.call(arguments, 1) : [];
                    return print(winner, runners);
                  };
                  if (typeof elvis != ""undefined"" && elvis !== null) {
                    alert(""I knew it!"");
                  }
                  cubes = (function() {
                    var _i, _len, _results;
                    _results = [];
                    for (_i = 0, _len = list.length; _i < _len; _i++) {
                      num = list[_i];
                      _results.push(math.cube(num));
                    }
                    return _results;
                  })();
                }).call(this);"),
               TestUtils.NormalizeText(engine.Evaluate<string>("CoffeeScript.compile(script, {})")));


            engine.SetGlobalValue("script", @"
                zappa = exports
                express = require 'express'
                fs = require 'fs'
                puts = console.log
                {inspect} = require 'sys'
                coffee = null
                jquery = null
                io = null
                coffeekup = null

                class Zappa
                  constructor: ->
                    @context = {}
                    @apps = {}
                    @current_app = null

                    @locals =
                      app: (name) => @app name
                      include: (path) => @include path
                      require: require
                      global: global
                      process: process
                      module: module

                    for name in 'get|post|put|del|route|at|msg|client|using|def|helper|postrender|layout|view|style'.split '|'
                      do (name) =>
                        @locals[name] = =>
                          @ensure_app 'default' unless @current_app?
                          @current_app[name].apply @current_app, arguments

                  app: (name) ->
                    @ensure_app name
                    @current_app = @apps[name]
  
                  include: (file) ->
                    @define_with @read_and_compile(file)
                    puts ""Included file \""#{file}\""""

                  define_with: (code) ->
                    scoped(code)(@context, @locals)

                  ensure_app: (name) ->
                    @apps[name] = new App(name) unless @apps[name]?
                    @current_app = @apps[name] unless @current_app?

                  read_and_compile: (file) ->
                    coffee = require 'coffee-script'
                    code = @read file
                    coffee.compile code
  
                  read: (file) -> fs.readFileSync file, 'utf8'
  
                  run_file: (file, options) ->
                    @locals.__filename = require('path').join(process.cwd(), file)
                    @locals.__dirname = process.cwd()
                    @locals.module.filename = @locals.__filename
                    code = if file.match /\.coffee$/ then @read_and_compile file else @read file
                    @run code, options
  
                  run: (code, options) ->
                    options ?= {}

                    @define_with code
    
                    i = 0
                    for k, a of @apps
                      opts = {}
                      if options.port
                        opts.port = if options.port[i]? then options.port[i] else a.port + i
                      else if i isnt 0
                        opts.port = a.port + i

                      opts.hostname = options.hostname if options.hostname

                      a.start opts
                      i++

                class App
                  constructor: (@name) ->
                    @name ?= 'default'
                    @port = 5678
    
                    @http_server = express.createServer()
                    if coffeekup?
                      @http_server.register '.coffee', coffeekup
                      @http_server.set 'view engine', 'coffee'
                    @http_server.configure =>
                      @http_server.use express.staticProvider(""#{process.cwd()}/public"")
                      @http_server.use express.bodyDecoder()
                      @http_server.use express.cookieDecoder()
                      # TODO: Make the secret configurable.
                      @http_server.use express.session(secret: 'hackme')

                    # App-level vars, exposed to handlers as [app].""
                    @vars = {}
    
                    @defs = {}
                    @helpers = {}
                    @postrenders = {}
                    @socket_handlers = {}
                    @msg_handlers = {}

                    @views = {}
                    @layouts = {}
                    @layouts.default = ->
                      doctype 5
                      html ->
                        head ->
                          title @title if @title
                          if @scripts
                            for s in @scripts
                              script src: s + '.js'
                          script(src: @script + '.js') if @script
                          if @stylesheets
                            for s in @stylesheets
                              link rel: 'stylesheet', href: s + '.css'
                          link(rel: 'stylesheet', href: @stylesheet + '.css') if @stylesheet
                          style @style if @style
                        body @content
    
                  start: (options) ->
                    options ?= {}
                    @port = options.port if options.port
                    @hostname = options.hostname if options.hostname

                    if io?
                      @ws_server = io.listen @http_server, {log: ->}
                      @ws_server.on 'connection', (client) =>
                        @socket_handlers.connection?.execute client
                        client.on 'disconnect', => @socket_handlers.disconnection?.execute client
                        client.on 'message', (raw_msg) =>
                          msg = parse_msg raw_msg
                          @msg_handlers[msg.title]?.execute client, msg.params

                    if @hostname? then @http_server.listen @port, @hostname
                    else @http_server.listen @port
    
                    puts ""App \""#{@name}\"" listening on #{if @hostname? then @hostname + ':' else '*:'}#{@port}...""
                    @http_server

                  get: -> @route 'get', arguments
                  post: -> @route 'post', arguments
                  put: -> @route 'put', arguments
                  del: -> @route 'del', arguments
                  route: (verb, args) ->
                    if typeof args[0] isnt 'object'
                      @register_route verb, args[0], args[1]
                    else
                      for k, v of args[0]
                        @register_route verb, k, v

                  register_route: (verb, path, response) ->
                    if typeof response isnt 'function'
                      @http_server[verb] path, (req, res) -> res.send String(response)
                    else
                      handler = new RequestHandler(response, @defs, @helpers, @postrenders, @views, @layouts, @vars)
                      @http_server[verb] path, (req, res, next) ->
                        handler.execute(req, res, next)

                  using: ->
                    pairs = {}
                    for a in arguments
                      pairs[a] = require(a)
                    @def pairs
   
                  def: (pairs) ->
                    for k, v of pairs
                      @defs[k] = v
   
                  helper: (pairs) ->
                    for k, v of pairs
                      @helpers[k] = scoped(v)

                  postrender: (pairs) ->
                    jquery = require 'jquery'
                    for k, v of pairs
                      @postrenders[k] = scoped(v)

                  at: (pairs) ->
                    io = require 'socket.io'
                    for k, v of pairs
                      @socket_handlers[k] = new MessageHandler(v, @)

                  msg: (pairs) ->
                    io = require 'socket.io'
                    for k, v of pairs
                      @msg_handlers[k] = new MessageHandler(v, @)

                  layout: (arg) ->
                    pairs = if typeof arg is 'object' then arg else {default: arg}
                    coffeekup = require 'coffeekup'
                    for k, v of pairs
                      @layouts[k] = v
   
                  view: (arg) ->
                    pairs = if typeof arg is 'object' then arg else {default: arg}
                    coffeekup = require 'coffeekup'
                    for k, v of pairs
                      @views[k] = v

                  client: (arg) ->
                    pairs = if typeof arg is 'object' then arg else {default: arg}
                    for k, v of pairs
                      do (k, v) =>
                        code = "";(#{v})();""
                        @http_server.get ""/#{k}.js"", (req, res) ->
                          res.contentType 'bla.js'
                          res.send code

                  style: (arg) ->
                    pairs = if typeof arg is 'object' then arg else {default: arg}
                    for k, v of pairs
                      do (k, v) =>
                        @http_server.get ""/#{k}.css"", (req, res) ->
                          res.contentType 'bla.css'
                          res.send v

                class RequestHandler
                  constructor: (handler, @defs, @helpers, @postrenders, @views, @layouts, @vars) ->
                    @handler = scoped(handler)
                    @locals = null

                  init_locals: ->
                    @locals = {}
                    @locals.app = @vars
                    @locals.render = @render
                    @locals.partial = @partial
                    @locals.redirect = @redirect
                    @locals.send = @send
                    @locals.puts = puts

                    for k, v of @defs
                      @locals[k] = v

                    for k, v of @helpers
                      do (k, v) =>
                        @locals[k] = ->
                          v(@context, @, arguments)

                    @locals.defs = @defs
                    @locals.postrenders = @postrenders
                    @locals.views = @views
                    @locals.layouts = @layouts

                  execute: (request, response, next) ->
                    @init_locals() unless @locals?

                    @locals.context = {}
                    @locals.params = @locals.context

                    @locals.request = request
                    @locals.response = response
                    @locals.next = next

                    @locals.session = request.session
                    @locals.cookies = request.cookies

                    for k, v of request.query
                      @locals.context[k] = v
                    for k, v of request.params
                      @locals.context[k] = v
                    for k, v of request.body
                      @locals.context[k] = v

                    result = @handler(@locals.context, @locals)

                    if typeof result is 'string'
                      response.send result
                    else
                      result

                  redirect: -> @response.redirect.apply @response, arguments
                  send: -> @response.send.apply @response, arguments

                  render: (template, options) ->
                    options ?= {}
                    options.layout ?= 'default'

                    opts = options.options or {} # Options for the templating engine.
                    opts.context ?= @context
                    opts.context.zappa = partial: @partial
                    opts.locals ?= {}
                    opts.locals.partial = (template, context) ->
                      text ck_options.context.zappa.partial template, context

                    template = @views[template] if typeof template is 'string'

                    result = coffeekup.render template, opts

                    if typeof options.apply is 'string'
                      postrender = @postrenders[options.apply]
                      body = jquery('body')
                      body.empty().html(result)
                      postrender opts.context, jquery.extend(@defs, {$: jquery})
                      result = body.html()

                    if options.layout
                      layout = @layouts[options.layout]
                      opts.context.content = result
                      result = coffeekup.render layout, opts

                    @response.send result

                    null

                  partial: (template, context) =>
                    template = @views[template]
                    coffeekup.render(template, context: context)

                class MessageHandler
                  constructor: (handler, @app) ->
                    @handler = scoped(handler)
                    @locals = null

                  init_locals: ->
                    @locals = {}
                    @locals.app = @app.vars
                    @locals.render = @render
                    @locals.partial = @partial
                    @locals.puts = puts
  
                    for k, v of @app.defs
                      @locals[k] = v

                    for k, v of @app.helpers
                      do (k, v) =>
                        @locals[k] = ->
                          v(@context, @, arguments)

                    @locals.defs = @app.defs
                    @locals.postrenders = @app.postrenders
                    @locals.views = @app.views
                    @locals.layouts = @app.layouts

                  execute: (client, params) ->
                    @init_locals() unless @locals?

                    @locals.context = {}
                    @locals.params = @locals.context
                    @locals.client = client
                    # TODO: Move this to context.
                    @locals.id = client.sessionId
                    @locals.send = (title, data) => client.send build_msg(title, data)
                    @locals.broadcast = (title, data, except) =>
                      except ?= []
                      if except not instanceof Array then except = [except]
                      except.push @locals.id
                      @app.ws_server.broadcast build_msg(title, data), except

                    for k, v of params
                      @locals.context[k] = v

                    @handler(@locals.context, @locals)

                  render: (template, options) ->
                    options ?= {}
                    options.layout ?= 'default'

                    opts = options.options or {} # Options for the templating engine.
                    opts.context ?= @context
                    opts.context.zappa = partial: @partial
                    opts.locals ?= {}
                    opts.locals.partial = (template, context) ->
                      text ck_options.context.zappa.partial template, context

                    template = @app.views[template] if typeof template is 'string'

                    result = coffeekup.render template, opts

                    if typeof options.apply is 'string'
                      postrender = @postrenders[options.apply]
                      body = jquery('body')
                      body.empty().html(result)
                      postrender opts.context, jquery.extend(@defs, {$: jquery})
                      result = body.html()

                    if options.layout
                      layout = @layouts[options.layout]
                      opts.context.content = result
                      result = coffeekup.render layout, opts

                    @send 'render', value: result

                    null

                  partial: (template, context) =>
                    template = @app.views[template]
                    coffeekup.render(template, context: context)

                coffeescript_support = """"""
                  var __slice = Array.prototype.slice;
                  var __hasProp = Object.prototype.hasOwnProperty;
                  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
                  var __extends = function(child, parent) {
                    for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; }
                    function ctor() { this.constructor = child; }
                    ctor.prototype = parent.prototype; child.prototype = new ctor; child.__super__ = parent.prototype;
                    return child;
                  };
                  var __indexOf = Array.prototype.indexOf || function(item) {
                    for (var i = 0, l = this.length; i < l; i++) {
                      if (this[i] === item) return i;
                    }
                    return -1;
                  };
                """"""

                build_msg = (title, data) ->
                  obj = {}
                  obj[title] = data
                  JSON.stringify(obj)

                parse_msg = (raw_msg) ->
                  obj = JSON.parse(raw_msg)
                  for k, v of obj
                    return {title: k, params: v}

                scoped = (code) ->
                  code = String(code)
                  code = ""function () {#{code}}"" unless code.indexOf('function') is 0
                  code = ""#{coffeescript_support} with(locals) {return (#{code}).apply(context, args);}""
                  new Function('context', 'locals', 'args', code)

                publish_api = (from, to, methods) ->
                  for name in methods.split '|'
                    do (name) ->
                      if typeof from[name] is 'function'
                        to[name] = -> from[name].apply from, arguments
                      else
                        to[name] = from[name]

                z = new Zappa()

                zappa.version = '0.1.4'
                zappa.run = -> z.run.apply z, arguments
                zappa.run_file = -> z.run_file.apply z, arguments");
            Assert.AreEqual(TestUtils.NormalizeText(@"
                (function() {
                  var App, MessageHandler, RequestHandler, Zappa, build_msg, coffee, coffeekup, coffeescript_support, express, fs, inspect, io, jquery, parse_msg, publish_api, puts, scoped, z, zappa;
                  var __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };
                  zappa = exports;
                  express = require('express');
                  fs = require('fs');
                  puts = console.log;
                  inspect = require('sys').inspect;
                  coffee = null;
                  jquery = null;
                  io = null;
                  coffeekup = null;
                  Zappa = (function() {
                    function Zappa() {
                      var name, _fn, _i, _len, _ref;
                      this.context = {};
                      this.apps = {};
                      this.current_app = null;
                      this.locals = {
                        app: __bind(function(name) {
                          return this.app(name);
                        }, this),
                        include: __bind(function(path) {
                          return this.include(path);
                        }, this),
                        require: require,
                        global: global,
                        process: process,
                        module: module
                      };
                      _ref = 'get|post|put|del|route|at|msg|client|using|def|helper|postrender|layout|view|style'.split('|');
                      _fn = __bind(function(name) {
                        return this.locals[name] = __bind(function() {
                          if (this.current_app == null) {
                            this.ensure_app('default');
                          }
                          return this.current_app[name].apply(this.current_app, arguments);
                        }, this);
                      }, this);
                      for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                        name = _ref[_i];
                        _fn(name);
                      }
                    }
                    Zappa.prototype.app = function(name) {
                      this.ensure_app(name);
                      return this.current_app = this.apps[name];
                    };
                    Zappa.prototype.include = function(file) {
                      this.define_with(this.read_and_compile(file));
                      return puts(""Included file \"""" + file + ""\"""");
                    };
                    Zappa.prototype.define_with = function(code) {
                      return scoped(code)(this.context, this.locals);
                    };
                    Zappa.prototype.ensure_app = function(name) {
                      if (this.apps[name] == null) {
                        this.apps[name] = new App(name);
                      }
                      if (this.current_app == null) {
                        return this.current_app = this.apps[name];
                      }
                    };
                    Zappa.prototype.read_and_compile = function(file) {
                      var code;
                      coffee = require('coffee-script');
                      code = this.read(file);
                      return coffee.compile(code);
                    };
                    Zappa.prototype.read = function(file) {
                      return fs.readFileSync(file, 'utf8');
                    };
                    Zappa.prototype.run_file = function(file, options) {
                      var code;
                      this.locals.__filename = require('path').join(process.cwd(), file);
                      this.locals.__dirname = process.cwd();
                      this.locals.module.filename = this.locals.__filename;
                      code = file.match(/\.coffee$/) ? this.read_and_compile(file) : this.read(file);
                      return this.run(code, options);
                    };
                    Zappa.prototype.run = function(code, options) {
                      var a, i, k, opts, _ref, _results;
                      options != null ? options : options = {};
                      this.define_with(code);
                      i = 0;
                      _ref = this.apps;
                      _results = [];
                      for (k in _ref) {
                        a = _ref[k];
                        opts = {};
                        if (options.port) {
                          opts.port = options.port[i] != null ? options.port[i] : a.port + i;
                        } else if (i !== 0) {
                          opts.port = a.port + i;
                        }
                        if (options.hostname) {
                          opts.hostname = options.hostname;
                        }
                        a.start(opts);
                        _results.push(i++);
                      }
                      return _results;
                    };
                    return Zappa;
                  })();
                  App = (function() {
                    function App(name) {
                      var _ref;
                      this.name = name;
                      (_ref = this.name) != null ? _ref : this.name = 'default';
                      this.port = 5678;
                      this.http_server = express.createServer();
                      if (coffeekup != null) {
                        this.http_server.register('.coffee', coffeekup);
                        this.http_server.set('view engine', 'coffee');
                      }
                      this.http_server.configure(__bind(function() {
                        this.http_server.use(express.staticProvider("""" + (process.cwd()) + ""/public""));
                        this.http_server.use(express.bodyDecoder());
                        this.http_server.use(express.cookieDecoder());
                        return this.http_server.use(express.session({
                          secret: 'hackme'
                        }));
                      }, this));
                      this.vars = {};
                      this.defs = {};
                      this.helpers = {};
                      this.postrenders = {};
                      this.socket_handlers = {};
                      this.msg_handlers = {};
                      this.views = {};
                      this.layouts = {};
                      this.layouts[""default""] = function() {
                        doctype(5);
                        return html(function() {
                          head(function() {
                            var s, _i, _j, _len, _len2, _ref, _ref2;
                            if (this.title) {
                              title(this.title);
                            }
                            if (this.scripts) {
                              _ref = this.scripts;
                              for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                                s = _ref[_i];
                                script({
                                  src: s + '.js'
                                });
                              }
                            }
                            if (this.script) {
                              script({
                                src: this.script + '.js'
                              });
                            }
                            if (this.stylesheets) {
                              _ref2 = this.stylesheets;
                              for (_j = 0, _len2 = _ref2.length; _j < _len2; _j++) {
                                s = _ref2[_j];
                                link({
                                  rel: 'stylesheet',
                                  href: s + '.css'
                                });
                              }
                            }
                            if (this.stylesheet) {
                              link({
                                rel: 'stylesheet',
                                href: this.stylesheet + '.css'
                              });
                            }
                            if (this.style) {
                              return style(this.style);
                            }
                          });
                          return body(this.content);
                        });
                      };
                    }
                    App.prototype.start = function(options) {
                      options != null ? options : options = {};
                      if (options.port) {
                        this.port = options.port;
                      }
                      if (options.hostname) {
                        this.hostname = options.hostname;
                      }
                      if (io != null) {
                        this.ws_server = io.listen(this.http_server, {
                          log: function() {}
                        });
                        this.ws_server.on('connection', __bind(function(client) {
                          var _ref;
                          if ((_ref = this.socket_handlers.connection) != null) {
                            _ref.execute(client);
                          }
                          client.on('disconnect', __bind(function() {
                            var _ref;
                            return (_ref = this.socket_handlers.disconnection) != null ? _ref.execute(client) : void 0;
                          }, this));
                          return client.on('message', __bind(function(raw_msg) {
                            var msg, _ref;
                            msg = parse_msg(raw_msg);
                            return (_ref = this.msg_handlers[msg.title]) != null ? _ref.execute(client, msg.params) : void 0;
                          }, this));
                        }, this));
                      }
                      if (this.hostname != null) {
                        this.http_server.listen(this.port, this.hostname);
                      } else {
                        this.http_server.listen(this.port);
                      }
                      puts(""App \"""" + this.name + ""\"" listening on "" + (this.hostname != null ? this.hostname + ':' : '*:') + this.port + ""..."");
                      return this.http_server;
                    };
                    App.prototype.get = function() {
                      return this.route('get', arguments);
                    };
                    App.prototype.post = function() {
                      return this.route('post', arguments);
                    };
                    App.prototype.put = function() {
                      return this.route('put', arguments);
                    };
                    App.prototype.del = function() {
                      return this.route('del', arguments);
                    };
                    App.prototype.route = function(verb, args) {
                      var k, v, _ref, _results;
                      if (typeof args[0] !== 'object') {
                        return this.register_route(verb, args[0], args[1]);
                      } else {
                        _ref = args[0];
                        _results = [];
                        for (k in _ref) {
                          v = _ref[k];
                          _results.push(this.register_route(verb, k, v));
                        }
                        return _results;
                      }
                    };
                    App.prototype.register_route = function(verb, path, response) {
                      var handler;
                      if (typeof response !== 'function') {
                        return this.http_server[verb](path, function(req, res) {
                          return res.send(String(response));
                        });
                      } else {
                        handler = new RequestHandler(response, this.defs, this.helpers, this.postrenders, this.views, this.layouts, this.vars);
                        return this.http_server[verb](path, function(req, res, next) {
                          return handler.execute(req, res, next);
                        });
                      }
                    };
                    App.prototype.using = function() {
                      var a, pairs, _i, _len;
                      pairs = {};
                      for (_i = 0, _len = arguments.length; _i < _len; _i++) {
                        a = arguments[_i];
                        pairs[a] = require(a);
                      }
                      return this.def(pairs);
                    };
                    App.prototype.def = function(pairs) {
                      var k, v, _results;
                      _results = [];
                      for (k in pairs) {
                        v = pairs[k];
                        _results.push(this.defs[k] = v);
                      }
                      return _results;
                    };
                    App.prototype.helper = function(pairs) {
                      var k, v, _results;
                      _results = [];
                      for (k in pairs) {
                        v = pairs[k];
                        _results.push(this.helpers[k] = scoped(v));
                      }
                      return _results;
                    };
                    App.prototype.postrender = function(pairs) {
                      var k, v, _results;
                      jquery = require('jquery');
                      _results = [];
                      for (k in pairs) {
                        v = pairs[k];
                        _results.push(this.postrenders[k] = scoped(v));
                      }
                      return _results;
                    };
                    App.prototype.at = function(pairs) {
                      var k, v, _results;
                      io = require('socket.io');
                      _results = [];
                      for (k in pairs) {
                        v = pairs[k];
                        _results.push(this.socket_handlers[k] = new MessageHandler(v, this));
                      }
                      return _results;
                    };
                    App.prototype.msg = function(pairs) {
                      var k, v, _results;
                      io = require('socket.io');
                      _results = [];
                      for (k in pairs) {
                        v = pairs[k];
                        _results.push(this.msg_handlers[k] = new MessageHandler(v, this));
                      }
                      return _results;
                    };
                    App.prototype.layout = function(arg) {
                      var k, pairs, v, _results;
                      pairs = typeof arg === 'object' ? arg : {
                        ""default"": arg
                      };
                      coffeekup = require('coffeekup');
                      _results = [];
                      for (k in pairs) {
                        v = pairs[k];
                        _results.push(this.layouts[k] = v);
                      }
                      return _results;
                    };
                    App.prototype.view = function(arg) {
                      var k, pairs, v, _results;
                      pairs = typeof arg === 'object' ? arg : {
                        ""default"": arg
                      };
                      coffeekup = require('coffeekup');
                      _results = [];
                      for (k in pairs) {
                        v = pairs[k];
                        _results.push(this.views[k] = v);
                      }
                      return _results;
                    };
                    App.prototype.client = function(arg) {
                      var k, pairs, v, _results;
                      pairs = typeof arg === 'object' ? arg : {
                        ""default"": arg
                      };
                      _results = [];
                      for (k in pairs) {
                        v = pairs[k];
                        _results.push(__bind(function(k, v) {
                          var code;
                          code = "";("" + v + "")();"";
                          return this.http_server.get(""/"" + k + "".js"", function(req, res) {
                            res.contentType('bla.js');
                            return res.send(code);
                          });
                        }, this)(k, v));
                      }
                      return _results;
                    };
                    App.prototype.style = function(arg) {
                      var k, pairs, v, _results;
                      pairs = typeof arg === 'object' ? arg : {
                        ""default"": arg
                      };
                      _results = [];
                      for (k in pairs) {
                        v = pairs[k];
                        _results.push(__bind(function(k, v) {
                          return this.http_server.get(""/"" + k + "".css"", function(req, res) {
                            res.contentType('bla.css');
                            return res.send(v);
                          });
                        }, this)(k, v));
                      }
                      return _results;
                    };
                    return App;
                  })();
                  RequestHandler = (function() {
                    function RequestHandler(handler, defs, helpers, postrenders, views, layouts, vars) {
                      this.defs = defs;
                      this.helpers = helpers;
                      this.postrenders = postrenders;
                      this.views = views;
                      this.layouts = layouts;
                      this.vars = vars;
                      this.partial = __bind(this.partial, this);;
                      this.handler = scoped(handler);
                      this.locals = null;
                    }
                    RequestHandler.prototype.init_locals = function() {
                      var k, v, _fn, _ref, _ref2;
                      this.locals = {};
                      this.locals.app = this.vars;
                      this.locals.render = this.render;
                      this.locals.partial = this.partial;
                      this.locals.redirect = this.redirect;
                      this.locals.send = this.send;
                      this.locals.puts = puts;
                      _ref = this.defs;
                      for (k in _ref) {
                        v = _ref[k];
                        this.locals[k] = v;
                      }
                      _ref2 = this.helpers;
                      _fn = __bind(function(k, v) {
                        return this.locals[k] = function() {
                          return v(this.context, this, arguments);
                        };
                      }, this);
                      for (k in _ref2) {
                        v = _ref2[k];
                        _fn(k, v);
                      }
                      this.locals.defs = this.defs;
                      this.locals.postrenders = this.postrenders;
                      this.locals.views = this.views;
                      return this.locals.layouts = this.layouts;
                    };
                    RequestHandler.prototype.execute = function(request, response, next) {
                      var k, result, v, _ref, _ref2, _ref3;
                      if (this.locals == null) {
                        this.init_locals();
                      }
                      this.locals.context = {};
                      this.locals.params = this.locals.context;
                      this.locals.request = request;
                      this.locals.response = response;
                      this.locals.next = next;
                      this.locals.session = request.session;
                      this.locals.cookies = request.cookies;
                      _ref = request.query;
                      for (k in _ref) {
                        v = _ref[k];
                        this.locals.context[k] = v;
                      }
                      _ref2 = request.params;
                      for (k in _ref2) {
                        v = _ref2[k];
                        this.locals.context[k] = v;
                      }
                      _ref3 = request.body;
                      for (k in _ref3) {
                        v = _ref3[k];
                        this.locals.context[k] = v;
                      }
                      result = this.handler(this.locals.context, this.locals);
                      if (typeof result === 'string') {
                        return response.send(result);
                      } else {
                        return result;
                      }
                    };
                    RequestHandler.prototype.redirect = function() {
                      return this.response.redirect.apply(this.response, arguments);
                    };
                    RequestHandler.prototype.send = function() {
                      return this.response.send.apply(this.response, arguments);
                    };
                    RequestHandler.prototype.render = function(template, options) {
                      var body, layout, opts, postrender, result, _ref, _ref2, _ref3;
                      options != null ? options : options = {};
                      (_ref = options.layout) != null ? _ref : options.layout = 'default';
                      opts = options.options || {};
                      (_ref2 = opts.context) != null ? _ref2 : opts.context = this.context;
                      opts.context.zappa = {
                        partial: this.partial
                      };
                      (_ref3 = opts.locals) != null ? _ref3 : opts.locals = {};
                      opts.locals.partial = function(template, context) {
                        return text(ck_options.context.zappa.partial(template, context));
                      };
                      if (typeof template === 'string') {
                        template = this.views[template];
                      }
                      result = coffeekup.render(template, opts);
                      if (typeof options.apply === 'string') {
                        postrender = this.postrenders[options.apply];
                        body = jquery('body');
                        body.empty().html(result);
                        postrender(opts.context, jquery.extend(this.defs, {
                          $: jquery
                        }));
                        result = body.html();
                      }
                      if (options.layout) {
                        layout = this.layouts[options.layout];
                        opts.context.content = result;
                        result = coffeekup.render(layout, opts);
                      }
                      this.response.send(result);
                      return null;
                    };
                    RequestHandler.prototype.partial = function(template, context) {
                      template = this.views[template];
                      return coffeekup.render(template, {
                        context: context
                      });
                    };
                    return RequestHandler;
                  })();
                  MessageHandler = (function() {
                    function MessageHandler(handler, app) {
                      this.app = app;
                      this.partial = __bind(this.partial, this);;
                      this.handler = scoped(handler);
                      this.locals = null;
                    }
                    MessageHandler.prototype.init_locals = function() {
                      var k, v, _fn, _ref, _ref2;
                      this.locals = {};
                      this.locals.app = this.app.vars;
                      this.locals.render = this.render;
                      this.locals.partial = this.partial;
                      this.locals.puts = puts;
                      _ref = this.app.defs;
                      for (k in _ref) {
                        v = _ref[k];
                        this.locals[k] = v;
                      }
                      _ref2 = this.app.helpers;
                      _fn = __bind(function(k, v) {
                        return this.locals[k] = function() {
                          return v(this.context, this, arguments);
                        };
                      }, this);
                      for (k in _ref2) {
                        v = _ref2[k];
                        _fn(k, v);
                      }
                      this.locals.defs = this.app.defs;
                      this.locals.postrenders = this.app.postrenders;
                      this.locals.views = this.app.views;
                      return this.locals.layouts = this.app.layouts;
                    };
                    MessageHandler.prototype.execute = function(client, params) {
                      var k, v;
                      if (this.locals == null) {
                        this.init_locals();
                      }
                      this.locals.context = {};
                      this.locals.params = this.locals.context;
                      this.locals.client = client;
                      this.locals.id = client.sessionId;
                      this.locals.send = __bind(function(title, data) {
                        return client.send(build_msg(title, data));
                      }, this);
                      this.locals.broadcast = __bind(function(title, data, except) {
                        except != null ? except : except = [];
                        if (!(except instanceof Array)) {
                          except = [except];
                        }
                        except.push(this.locals.id);
                        return this.app.ws_server.broadcast(build_msg(title, data), except);
                      }, this);
                      for (k in params) {
                        v = params[k];
                        this.locals.context[k] = v;
                      }
                      return this.handler(this.locals.context, this.locals);
                    };
                    MessageHandler.prototype.render = function(template, options) {
                      var body, layout, opts, postrender, result, _ref, _ref2, _ref3;
                      options != null ? options : options = {};
                      (_ref = options.layout) != null ? _ref : options.layout = 'default';
                      opts = options.options || {};
                      (_ref2 = opts.context) != null ? _ref2 : opts.context = this.context;
                      opts.context.zappa = {
                        partial: this.partial
                      };
                      (_ref3 = opts.locals) != null ? _ref3 : opts.locals = {};
                      opts.locals.partial = function(template, context) {
                        return text(ck_options.context.zappa.partial(template, context));
                      };
                      if (typeof template === 'string') {
                        template = this.app.views[template];
                      }
                      result = coffeekup.render(template, opts);
                      if (typeof options.apply === 'string') {
                        postrender = this.postrenders[options.apply];
                        body = jquery('body');
                        body.empty().html(result);
                        postrender(opts.context, jquery.extend(this.defs, {
                          $: jquery
                        }));
                        result = body.html();
                      }
                      if (options.layout) {
                        layout = this.layouts[options.layout];
                        opts.context.content = result;
                        result = coffeekup.render(layout, opts);
                      }
                      this.send('render', {
                        value: result
                      });
                      return null;
                    };
                    MessageHandler.prototype.partial = function(template, context) {
                      template = this.app.views[template];
                      return coffeekup.render(template, {
                        context: context
                      });
                    };
                    return MessageHandler;
                  })();
                  coffeescript_support = ""var __slice = Array.prototype.slice;\nvar __hasProp = Object.prototype.hasOwnProperty;\nvar __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };\nvar __extends = function(child, parent) {\n  for (var key in parent) { if (__hasProp.call(parent, key)) child[key] = parent[key]; }\n  function ctor() { this.constructor = child; }\n  ctor.prototype = parent.prototype; child.prototype = new ctor; child.__super__ = parent.prototype;\n  return child;\n};\nvar __indexOf = Array.prototype.indexOf || function(item) {\n  for (var i = 0, l = this.length; i < l; i++) {\n    if (this[i] === item) return i;\n  }\n  return -1;\n};"";
                  build_msg = function(title, data) {
                    var obj;
                    obj = {};
                    obj[title] = data;
                    return JSON.stringify(obj);
                  };
                  parse_msg = function(raw_msg) {
                    var k, obj, v;
                    obj = JSON.parse(raw_msg);
                    for (k in obj) {
                      v = obj[k];
                      return {
                        title: k,
                        params: v
                      };
                    }
                  };
                  scoped = function(code) {
                    code = String(code);
                    if (code.indexOf('function') !== 0) {
                      code = ""function () {"" + code + ""}"";
                    }
                    code = """" + coffeescript_support + "" with(locals) {return ("" + code + "").apply(context, args);}"";
                    return new Function('context', 'locals', 'args', code);
                  };
                  publish_api = function(from, to, methods) {
                    var name, _i, _len, _ref, _results;
                    _ref = methods.split('|');
                    _results = [];
                    for (_i = 0, _len = _ref.length; _i < _len; _i++) {
                      name = _ref[_i];
                      _results.push((function(name) {
                        if (typeof from[name] === 'function') {
                          return to[name] = function() {
                            return from[name].apply(from, arguments);
                          };
                        } else {
                          return to[name] = from[name];
                        }
                      })(name));
                    }
                    return _results;
                  };
                  z = new Zappa();
                  zappa.version = '0.1.4';
                  zappa.run = function() {
                    return z.run.apply(z, arguments);
                  };
                  zappa.run_file = function() {
                    return z.run_file.apply(z, arguments);
                  };
                }).call(this);"),
               TestUtils.NormalizeText(engine.Evaluate<string>("CoffeeScript.compile(script, {})")));
            System.IO.File.WriteAllText(@"c:\users\paul\actual.txt", TestUtils.NormalizeText(engine.Evaluate<string>("CoffeeScript.compile(script, {})")));
        }

        [TestMethod]
        public void NoAlphaNumeric()
        {
            // From http://discogscounter.getfreehosting.co.uk/js-noalnum_com.php
            TestUtils.Execute("var sort = Array.prototype.sort; Array.prototype.sort = function() { return 'obfuscated'; }");
            try
            {
                Assert.AreEqual("obfuscated", TestUtils.Evaluate("($=[$=[]][(__=!$+$)[_=-~-~-~$]+({}+$)[_/_]+($$=($_=!''+$)[_/_]+$_[+$])])()"));
            }
            finally
            {
                TestUtils.Execute("Array.prototype.sort = sort");
            }
        }
    }
}
