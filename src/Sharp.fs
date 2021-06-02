// ts2fable 0.7.1
module rec Sharp
open System
open Fable.Core
open Fable.Core.JS

type Error = System.Exception

type ArrayLike<'t> =
    abstract Item: int -> 't with get

type Record<'key, 'value> =
    abstract Item: 'key -> 'value with get

let [<ImportDefault("sharp")>] sharp: IExports = jsNative

type [<AllowNullLiteral>] IExports =
    /// <summary>Creates a sharp instance from an image</summary>
    /// <param name="options">Object with optional attributes.</param>
    [<Emit("$0($1...)")>]
    abstract Create: input: string * ?options: SharpOptions -> Sharp
    /// <summary>Creates a sharp instance from an image</summary>
    /// <param name="options">Object with optional attributes.</param>
    [<Emit("$0($1...)")>]
    abstract Create: input: Node.Buffer.Buffer * ?options: SharpOptions -> Sharp
    [<Emit("$0($1...)")>]
    abstract Create: ?options: SharpOptions -> Sharp

    abstract format: FormatEnum
    abstract versions: IExportsVersions
    abstract queue: Node.Events.EventEmitter
    /// <summary>Gets or, when options are provided, sets the limits of libvips' operation cache.
    /// Existing entries in the cache will be trimmed after any change in limits.
    /// This method always returns cache statistics, useful for determining how much working memory is required for a particular task.</summary>
    /// <param name="options">Object with the following attributes, or Boolean where true uses default cache settings and false removes all caching (optional, default true)</param>
    abstract cache: ?options: U2<bool, CacheOptions> -> CacheResult
    /// <summary>Gets or sets the number of threads libvips' should create to process each image.
    /// The default value is the number of CPU cores. A value of 0 will reset to this default.
    /// The maximum number of images that can be processed in parallel is limited by libuv's UV_THREADPOOL_SIZE environment variable.</summary>
    /// <param name="concurrency">The new concurrency value.</param>
    abstract concurrency: ?concurrency: float -> float
    /// Provides access to internal task counters.
    abstract counters: unit -> SharpCounters
    /// <summary>Get and set use of SIMD vector unit instructions. Requires libvips to have been compiled with liborc support.
    /// Improves the performance of resize, blur and sharpen operations by taking advantage of the SIMD vector unit of the CPU, e.g. Intel SSE and ARM NEON.</summary>
    /// <param name="enable">enable or disable use of SIMD vector unit instructions</param>
    abstract simd: ?enable: bool -> bool
    abstract gravity: GravityEnum
    abstract strategy: StrategyEnum
    abstract kernel: KernelEnum
    abstract fit: FitEnum
    abstract bool: BoolEnum

type [<AllowNullLiteral>] Sharp =
    inherit Node.Stream.Duplex<U2<string,Buffer>, U2<string,Buffer>>
    /// Remove alpha channel, if any. This is a no-op if the image does not have an alpha channel.
    abstract removeAlpha: unit -> Sharp
    /// <summary>Ensure alpha channel, if missing. The added alpha channel will be fully opaque. This is a no-op if the image already has an alpha channel.</summary>
    /// <param name="alpha">transparency level (0=fully-transparent, 1=fully-opaque) (optional, default 1).</param>
    abstract ensureAlpha: ?alpha: float -> Sharp
    /// <summary>Extract a single channel from a multi-channel image.</summary>
    /// <param name="channel">zero-indexed band number to extract, or red, green or blue as alternative to 0, 1 or 2 respectively.</param>
    abstract extractChannel: channel: U2<float, string> -> Sharp
    /// <summary>Join one or more channels to the image. The meaning of the added channels depends on the output colourspace, set with toColourspace().
    /// By default the output image will be web-friendly sRGB, with additional channels interpreted as alpha channels. Channel ordering follows vips convention:
    ///   - sRGB: 0: Red, 1: Green, 2: Blue, 3: Alpha.
    ///   - CMYK: 0: Magenta, 1: Cyan, 2: Yellow, 3: Black, 4: Alpha.
    ///
    /// Buffers may be any of the image formats supported by sharp.
    /// For raw pixel input, the options object should contain a raw attribute, which follows the format of the attribute of the same name in the sharp() constructor.</summary>
    /// <param name="images">one or more images (file paths, Buffers).</param>
    /// <param name="options">image options, see sharp() constructor.</param>
    abstract joinChannel: images: U3<string, Buffer, ArrayLike<U2<string, Buffer>>> * ?options: SharpOptions -> Sharp
    /// <summary>Perform a bitwise boolean operation on all input image channels (bands) to produce a single channel output image.</summary>
    /// <param name="boolOp">one of "and", "or" or "eor" to perform that bitwise operation.</param>
    abstract bandbool: boolOp: string -> Sharp
    /// <summary>Tint the image using the provided chroma while preserving the image luminance.
    /// An alpha channel may be present and will be unchanged by the operation.</summary>
    /// <param name="rgb">Parsed by the color module to extract chroma values.</param>
    abstract tint: rgb: Color -> Sharp
    /// <summary>Convert to 8-bit greyscale; 256 shades of grey.
    /// This is a linear operation.
    /// If the input image is in a non-linear colour space such as sRGB, use gamma() with greyscale() for the best results.
    /// By default the output image will be web-friendly sRGB and contain three (identical) color channels.
    /// This may be overridden by other sharp operations such as toColourspace('b-w'), which will produce an output image containing one color channel.
    /// An alpha channel may be present, and will be unchanged by the operation.</summary>
    /// <param name="greyscale">true to enable and false to disable (defaults to true)</param>
    abstract greyscale: ?greyscale: bool -> Sharp
    /// <summary>Alternative spelling of greyscale().</summary>
    /// <param name="grayscale">true to enable and false to disable (defaults to true)</param>
    abstract grayscale: ?grayscale: bool -> Sharp
    /// <summary>Set the output colourspace.
    /// By default output image will be web-friendly sRGB, with additional channels interpreted as alpha channels.</summary>
    /// <param name="colourspace">output colourspace e.g. srgb, rgb, cmyk, lab, b-w ...</param>
    abstract toColourspace: ?colourspace: string -> Sharp
    /// <summary>Alternative spelling of toColourspace().</summary>
    /// <param name="colorspace">output colorspace e.g. srgb, rgb, cmyk, lab, b-w ...</param>
    abstract toColorspace: colorspace: string -> Sharp
    /// <summary>Composite image(s) over the processed (resized, extracted etc.) image.
    ///
    /// The images to composite must be the same size or smaller than the processed image.
    /// If both `top` and `left` options are provided, they take precedence over `gravity`.</summary>
    /// <param name="images">- Ordered list of images to composite</param>
    abstract composite: images: ResizeArray<OverlayOptions> -> Sharp
    /// Take a "snapshot" of the Sharp instance, returning a new instance.
    /// Cloned instances inherit the input of their parent instance.
    /// This allows multiple output Streams and therefore multiple processing pipelines to share a single input Stream.
    abstract clone: unit -> Sharp
    /// Fast access to (uncached) image metadata without decoding any compressed image data.
    abstract metadata: callback: (Error -> Metadata -> unit) -> Sharp
    /// Fast access to (uncached) image metadata without decoding any compressed image data.
    abstract metadata: unit -> Promise<Metadata>
    /// Access to pixel-derived image statistics for every channel in the image.
    abstract stats: callback: (Error -> Stats -> unit) -> Sharp
    /// Access to pixel-derived image statistics for every channel in the image.
    abstract stats: unit -> Promise<Stats>
    /// <summary>Rotate the output image by either an explicit angle or auto-orient based on the EXIF Orientation tag.
    ///
    /// If an angle is provided, it is converted to a valid positive degree rotation. For example, -450 will produce a 270deg rotation.
    ///
    /// When rotating by an angle other than a multiple of 90, the background colour can be provided with the background option.
    ///
    /// If no angle is provided, it is determined from the EXIF data. Mirroring is supported and may infer the use of a flip operation.
    ///
    /// The use of rotate implies the removal of the EXIF Orientation tag, if any.
    ///
    /// Method order is important when both rotating and extracting regions, for example rotate(x).extract(y) will produce a different result to extract(y).rotate(x).</summary>
    /// <param name="angle">angle of rotation. (optional, default auto)</param>
    /// <param name="options">if present, is an Object with optional attributes.</param>
    abstract rotate: ?angle: float * ?options: RotateOptions -> Sharp
    /// <summary>Flip the image about the vertical Y axis. This always occurs after rotation, if any.
    /// The use of flip implies the removal of the EXIF Orientation tag, if any.</summary>
    /// <param name="flip">true to enable and false to disable (defaults to true)</param>
    abstract flip: ?flip: bool -> Sharp
    /// <summary>Flop the image about the horizontal X axis. This always occurs after rotation, if any.
    /// The use of flop implies the removal of the EXIF Orientation tag, if any.</summary>
    /// <param name="flop">true to enable and false to disable (defaults to true)</param>
    abstract flop: ?flop: bool -> Sharp
    /// <summary>Sharpen the image.
    /// When used without parameters, performs a fast, mild sharpen of the output image.
    /// When a sigma is provided, performs a slower, more accurate sharpen of the L channel in the LAB colour space.
    /// Separate control over the level of sharpening in "flat" and "jagged" areas is available.</summary>
    /// <param name="sigma">the sigma of the Gaussian mask, where sigma = 1 + radius / 2.</param>
    /// <param name="flat">the level of sharpening to apply to "flat" areas. (optional, default 1.0)</param>
    /// <param name="jagged">the level of sharpening to apply to "jagged" areas. (optional, default 2.0)</param>
    abstract sharpen: ?sigma: float * ?flat: float * ?jagged: float -> Sharp
    /// <summary>Apply median filter. When used without parameters the default window is 3x3.</summary>
    /// <param name="size">square mask size: size x size (optional, default 3)</param>
    abstract median: ?size: float -> Sharp
    /// <summary>Blur the image.
    /// When used without parameters, performs a fast, mild blur of the output image.
    /// When a sigma is provided, performs a slower, more accurate Gaussian blur.
    /// When a boolean sigma is provided, ether blur mild or disable blur</summary>
    /// <param name="sigma">a value between 0.3 and 1000 representing the sigma of the Gaussian mask, where sigma = 1 + radius / 2.</param>
    abstract blur: ?sigma: U2<float, bool> -> Sharp
    /// <summary>Merge alpha transparency channel, if any, with background.</summary>
    /// <param name="flatten">true to enable and false to disable (defaults to true)</param>
    abstract flatten: ?flatten: U2<bool, FlattenOptions> -> Sharp
    /// <summary>Apply a gamma correction by reducing the encoding (darken) pre-resize at a factor of 1/gamma then increasing the encoding (brighten) post-resize at a factor of gamma.
    /// This can improve the perceived brightness of a resized image in non-linear colour spaces.
    /// JPEG and WebP input images will not take advantage of the shrink-on-load performance optimisation when applying a gamma correction.</summary>
    /// <param name="gamma">value between 1.0 and 3.0. (optional, default 2.2)</param>
    abstract gamma: ?gamma: float -> Sharp
    /// <summary>Produce the "negative" of the image.</summary>
    /// <param name="negate">true to enable and false to disable (defaults to true)</param>
    abstract negate: ?negate: bool -> Sharp
    /// <summary>Enhance output image contrast by stretching its luminance to cover the full dynamic range.</summary>
    /// <param name="normalise">true to enable and false to disable (defaults to true)</param>
    abstract normalise: ?normalise: bool -> Sharp
    /// <summary>Alternative spelling of normalise.</summary>
    /// <param name="normalize">true to enable and false to disable (defaults to true)</param>
    abstract normalize: ?normalize: bool -> Sharp
    /// <summary>Perform contrast limiting adaptive histogram equalization (CLAHE)
    ///
    /// This will, in general, enhance the clarity of the image by bringing out
    /// darker details. Please read more about CLAHE here:
    /// https://en.wikipedia.org/wiki/Adaptive_histogram_equalization#Contrast_Limited_AHE</summary>
    /// <param name="options">clahe options</param>
    abstract clahe: options: ClaheOptions -> Sharp
    /// <summary>Convolve the image with the specified kernel.</summary>
    /// <param name="kernel">the specified kernel</param>
    abstract convolve: kernel: Kernel -> Sharp
    /// <summary>Any pixel value greather than or equal to the threshold value will be set to 255, otherwise it will be set to 0.</summary>
    /// <param name="threshold">a value in the range 0-255 representing the level at which the threshold will be applied. (optional, default 128)</param>
    /// <param name="options">threshold options</param>
    abstract threshold: ?threshold: float * ?options: ThresholdOptions -> Sharp
    /// <summary>Perform a bitwise boolean operation with operand image.
    /// This operation creates an output image where each pixel is the result of the selected bitwise boolean operation between the corresponding pixels of the input images.</summary>
    /// <param name="operand">Buffer containing image data or String containing the path to an image file.</param>
    /// <param name="operator">one of "and", "or" or "eor" to perform that bitwise operation.</param>
    /// <param name="options">describes operand when using raw pixel data.</param>
    abstract boolean: operand: U2<string, Buffer> * operator: string * ?options: SharpBooleanOptions -> Sharp
    /// <summary>Apply the linear formula a * input + b to the image (levels adjustment)</summary>
    /// <param name="a">multiplier (optional, default 1.0)</param>
    /// <param name="b">offset (optional, default 0.0)</param>
    abstract linear: ?a: float * ?b: float -> Sharp
    /// <summary>Recomb the image with the specified matrix.</summary>
    /// <param name="inputMatrix">3x3 Recombination matrix</param>
    abstract recomb: inputMatrix: Matrix3x3 -> Sharp
    /// <summary>Transforms the image using brightness, saturation and hue rotation.</summary>
    /// <param name="options">describes the modulation</param>
    abstract modulate: ?options: SharpModulateOptions -> Sharp
    /// <summary>Write output image data to a file.
    /// If an explicit output format is not selected, it will be inferred from the extension, with JPEG, PNG, WebP, AVIF, TIFF, DZI, and libvips' V format supported.
    /// Note that raw pixel data is only supported for buffer output.</summary>
    /// <param name="fileOut">The path to write the image data to.</param>
    /// <param name="callback">Callback function called on completion with two arguments (err, info).  info contains the output image format, size (bytes), width, height and channels.</param>
    abstract toFile: fileOut: string * callback: (Error -> OutputInfo -> unit) -> Sharp
    /// <summary>Write output image data to a file.</summary>
    /// <param name="fileOut">The path to write the image data to.</param>
    abstract toFile: fileOut: string -> Promise<OutputInfo>
    /// <summary>Write output to a Buffer. JPEG, PNG, WebP, AVIF, TIFF and RAW output are supported.
    /// By default, the format will match the input image, except GIF and SVG input which become PNG output.</summary>
    /// <param name="callback">Callback function called on completion with three arguments (err, buffer, info).</param>
    abstract toBuffer: callback: (Error -> Buffer -> OutputInfo -> unit) -> Sharp
    /// <summary>Write output to a Buffer. JPEG, PNG, WebP, AVIF, TIFF and RAW output are supported.
    /// By default, the format will match the input image, except GIF and SVG input which become PNG output.</summary>
    /// <param name="options">resolve options</param>
    abstract toBuffer: ?options: SharpToBufferOptions -> Promise<Buffer>
    /// <summary>Write output to a Buffer. JPEG, PNG, WebP, AVIF, TIFF and RAW output are supported.
    /// By default, the format will match the input image, except GIF and SVG input which become PNG output.</summary>
    /// <param name="options">resolve options</param>
    abstract toBuffer: options: SharpToBufferOptions_ -> Promise<SharpToBufferPromise>
    /// Include all metadata (EXIF, XMP, IPTC) from the input image in the output image.
    /// The default behaviour, when withMetadata is not used, is to strip all metadata and convert to the device-independent sRGB colour space.
    /// This will also convert to and add a web-friendly sRGB ICC profile.
    abstract withMetadata: ?withMetadata: WriteableMetadata -> Sharp
    /// <summary>Use these JPEG options for output image.</summary>
    /// <param name="options">Output options.</param>
    abstract jpeg: ?options: JpegOptions -> Sharp
    /// <summary>Use these PNG options for output image.
    /// PNG output is always full colour at 8 or 16 bits per pixel.
    /// Indexed PNG input at 1, 2 or 4 bits per pixel is converted to 8 bits per pixel.</summary>
    /// <param name="options">Output options.</param>
    abstract png: ?options: PngOptions -> Sharp
    /// <summary>Use these WebP options for output image.</summary>
    /// <param name="options">Output options.</param>
    abstract webp: ?options: WebpOptions -> Sharp
    /// <summary>Use these AVIF options for output image.
    /// Whilst it is possible to create AVIF images smaller than 16x16 pixels, most web browsers do not display these properly.</summary>
    /// <param name="options">Output options.</param>
    abstract avif: ?options: AvifOptions -> Sharp
    /// <summary>Use these HEIF options for output image.
    /// Support for patent-encumbered HEIC images requires the use of a globally-installed libvips compiled with support for libheif, libde265 and x265.</summary>
    /// <param name="options">Output options.</param>
    abstract heif: ?options: HeifOptions -> Sharp
    /// <summary>Use these TIFF options for output image.</summary>
    /// <param name="options">Output options.</param>
    abstract tiff: ?options: TiffOptions -> Sharp
    /// Force output to be raw, uncompressed uint8 pixel data.
    abstract raw: unit -> Sharp
    /// <summary>Force output to a given format.</summary>
    /// <param name="format">a String or an Object with an 'id' attribute</param>
    /// <param name="options">output options</param>
    abstract toFormat: format: U2<FormatEnum, AvailableFormatInfo> * ?options: U8<OutputOptions, JpegOptions, PngOptions, WebpOptions, AvifOptions, HeifOptions, GifOptions, TiffOptions> -> Sharp
    /// <summary>Use tile-based deep zoom (image pyramid) output.
    /// Set the format and options for tile images via the toFormat, jpeg, png or webp functions.
    /// Use a .zip or .szi file extension with toFile to write to a compressed archive file format.
    ///
    /// Warning: multiple sharp instances concurrently producing tile output can expose a possible race condition in some versions of libgsf.</summary>
    /// <param name="tile">tile options</param>
    abstract tile: ?tile: TileOptions -> Sharp
    /// <summary>Resize image to width, height or width x height.
    ///
    /// When both a width and height are provided, the possible methods by which the image should fit these are:
    ///   - cover: Crop to cover both provided dimensions (the default).
    ///   - contain: Embed within both provided dimensions.
    ///   - fill: Ignore the aspect ratio of the input and stretch to both provided dimensions.
    ///   - inside: Preserving aspect ratio, resize the image to be as large as possible while ensuring its dimensions are less than or equal to both those specified.
    ///   - outside: Preserving aspect ratio, resize the image to be as small as possible while ensuring its dimensions are greater than or equal to both those specified.
    ///              Some of these values are based on the object-fit CSS property.
    ///
    /// When using a fit of cover or contain, the default position is centre. Other options are:
    ///   - sharp.position: top, right top, right, right bottom, bottom, left bottom, left, left top.
    ///   - sharp.gravity: north, northeast, east, southeast, south, southwest, west, northwest, center or centre.
    ///   - sharp.strategy: cover only, dynamically crop using either the entropy or attention strategy. Some of these values are based on the object-position CSS property.
    ///
    /// The experimental strategy-based approach resizes so one dimension is at its target length then repeatedly ranks edge regions,
    /// discarding the edge with the lowest score based on the selected strategy.
    ///   - entropy: focus on the region with the highest Shannon entropy.
    ///   - attention: focus on the region with the highest luminance frequency, colour saturation and presence of skin tones.
    ///
    /// Possible interpolation kernels are:
    ///   - nearest: Use nearest neighbour interpolation.
    ///   - cubic: Use a Catmull-Rom spline.
    ///   - lanczos2: Use a Lanczos kernel with a=2.
    ///   - lanczos3: Use a Lanczos kernel with a=3 (the default).</summary>
    /// <param name="width">pixels wide the resultant image should be. Use null or undefined to auto-scale the width to match the height.</param>
    /// <param name="height">pixels high the resultant image should be. Use null or undefined to auto-scale the height to match the width.</param>
    /// <param name="options">resize options</param>
    abstract resize: ?width: float * ?height: float * ?options: ResizeOptions -> Sharp
    /// <summary>Shorthand for resize(null, null, options);</summary>
    /// <param name="options">resize options</param>
    abstract resize: options: ResizeOptions -> Sharp
    /// <summary>Extends/pads the edges of the image with the provided background colour.
    /// This operation will always occur after resizing and extraction, if any.</summary>
    /// <param name="extend">single pixel count to add to all edges or an Object with per-edge counts</param>
    abstract extend: extend: U2<float, ExtendOptions> -> Sharp
    /// <summary>Extract a region of the image.
    ///   - Use extract() before resize() for pre-resize extraction.
    ///   - Use extract() after resize() for post-resize extraction.
    ///   - Use extract() before and after for both.</summary>
    /// <param name="region">The region to extract</param>
    abstract extract: region: Region -> Sharp
    /// <summary>Trim "boring" pixels from all edges that contain values similar to the top-left pixel.
    /// The info response Object will contain trimOffsetLeft and trimOffsetTop properties.</summary>
    /// <param name="threshold">The allowed difference from the top-left pixel, a number greater than zero. (optional, default 10)</param>
    abstract trim: ?threshold: float -> Sharp

type [<AllowNullLiteral>] SharpBooleanOptions =
    abstract raw: Raw with get, set

type [<AllowNullLiteral>] SharpModulateOptions =
    abstract brightness: float option with get, set
    abstract saturation: float option with get, set
    abstract hue: float option with get, set

type [<AllowNullLiteral>] SharpToBufferOptions =
    abstract resolveWithObject: obj with get, set

type [<AllowNullLiteral>] SharpToBufferOptions_ =
    abstract resolveWithObject: obj with get, set

type [<AllowNullLiteral>] SharpOptions =
    /// By default halt processing and raise an error when loading invalid images.
    /// Set this flag to false if you'd rather apply a "best effort" to decode images,
    /// even if the data is corrupt or invalid. (optional, default true)
    /// (optional, default true)
    abstract failOnError: bool option with get, set
    /// Do not process input images where the number of pixels (width x height) exceeds this limit.
    /// Assumes image dimensions contained in the input metadata can be trusted.
    /// An integral Number of pixels, zero or false to remove limit, true to use default limit of 268402689 (0x3FFF x 0x3FFF). (optional, default 268402689)
    abstract limitInputPixels: U2<float, bool> option with get, set
    /// Set this to true to use sequential rather than random access where possible. This can reduce memory usage and might improve performance on some systems. (optional, default false)
    abstract sequentialRead: bool option with get, set
    /// Number representing the DPI for vector images. (optional, default 72)
    abstract density: float option with get, set
    /// Number of pages to extract for multi-page input (GIF, TIFF, PDF), use -1 for all pages
    abstract pages: float option with get, set
    /// Page number to start extracting from for multi-page input (GIF, TIFF, PDF), zero based. (optional, default 0)
    abstract page: float option with get, set
    /// Level to extract from a multi-level input (OpenSlide), zero based. (optional, default 0)
    abstract level: float option with get, set
    /// Set to `true` to read all frames/pages of an animated image (equivalent of setting `pages` to `-1`). (optional, default false)
    abstract animated: bool option with get, set
    /// Describes raw pixel input image data. See raw() for pixel ordering.
    abstract raw: Raw option with get, set
    /// Describes a new image to be created.
    abstract create: Create option with get, set

type [<AllowNullLiteral>] CacheOptions =
    /// Is the maximum memory in MB to use for this cache (optional, default 50)
    abstract memory: float option with get, set
    /// Is the maximum number of files to hold open (optional, default 20)
    abstract files: float option with get, set
    /// Is the maximum number of operations to cache (optional, default 100)
    abstract items: float option with get, set

type [<AllowNullLiteral>] SharpCounters =
    /// The number of tasks this module has queued waiting for libuv to provide a worker thread from its pool.
    abstract queue: float with get, set
    /// The number of resize tasks currently being processed.
    abstract ``process``: float with get, set

type [<AllowNullLiteral>] Raw =
    abstract width: float with get, set
    abstract height: float with get, set
    abstract channels: RawChannels with get, set

type [<AllowNullLiteral>] Create =
    /// Number of pixels wide.
    abstract width: float with get, set
    /// Number of pixels high.
    abstract height: float with get, set
    /// Number of bands e.g. 3 for RGB, 4 for RGBA
    abstract channels: Channels with get, set
    /// Parsed by the [color](https://www.npmjs.org/package/color) module to extract values for red, green, blue and alpha.
    abstract background: Color with get, set

type [<AllowNullLiteral>] WriteableMetadata =
    /// Value between 1 and 8, used to update the EXIF Orientation tag.
    abstract orientation: float option with get, set
    /// Filesystem path to output ICC profile, defaults to sRGB.
    abstract icc: string option with get, set
    /// Object keyed by IFD0, IFD1 etc. of key/value string pairs to write as EXIF data. (optional, default {})
    abstract exif: Record<string, obj option> option with get, set
    /// Number of pixels per inch (DPI)
    abstract density: float option with get, set

type [<AllowNullLiteral>] Metadata =
    /// Number value of the EXIF Orientation header, if present
    abstract orientation: float option with get, set
    /// Name of decoder used to decompress image data e.g. jpeg, png, webp, gif, svg
    abstract format: FormatEnum option with get, set
    /// Total size of image in bytes, for Stream and Buffer input only
    abstract size: float option with get, set
    /// Number of pixels wide (EXIF orientation is not taken into consideration)
    abstract width: float option with get, set
    /// Number of pixels high (EXIF orientation is not taken into consideration)
    abstract height: float option with get, set
    /// Name of colour space interpretation
    abstract space: ColourspaceEnum option with get, set
    /// Number of bands e.g. 3 for sRGB, 4 for CMYK
    abstract channels: Channels option with get, set
    /// Name of pixel depth format e.g. uchar, char, ushort, float ...
    abstract depth: string option with get, set
    /// Number of pixels per inch (DPI), if present
    abstract density: float option with get, set
    /// String containing JPEG chroma subsampling, 4:2:0 or 4:4:4 for RGB, 4:2:0:4 or 4:4:4:4 for CMYK
    abstract chromaSubsampling: string with get, set
    /// Boolean indicating whether the image is interlaced using a progressive scan
    abstract isProgressive: bool option with get, set
    /// Number of pages/frames contained within the image, with support for TIFF, HEIF, PDF, animated GIF and animated WebP
    abstract pages: float option with get, set
    /// Number of pixels high each page in a multi-page image will be.
    abstract pageHeight: float option with get, set
    /// Number of times to loop an animated image, zero refers to a continuous loop.
    abstract loop: float option with get, set
    /// Delay in ms between each page in an animated image, provided as an array of integers.
    abstract delay: ResizeArray<float> option with get, set
    /// Number of the primary page in a HEIF image
    abstract pagePrimary: float option with get, set
    /// Boolean indicating the presence of an embedded ICC profile
    abstract hasProfile: bool option with get, set
    /// Boolean indicating the presence of an alpha transparency channel
    abstract hasAlpha: bool option with get, set
    /// Buffer containing raw EXIF data, if present
    abstract exif: Buffer option with get, set
    /// Buffer containing raw ICC profile data, if present
    abstract icc: Buffer option with get, set
    /// Buffer containing raw IPTC data, if present
    abstract iptc: Buffer option with get, set
    /// Buffer containing raw XMP data, if present
    abstract xmp: Buffer option with get, set
    /// Buffer containing raw TIFFTAG_PHOTOSHOP data, if present
    abstract tifftagPhotoshop: Buffer option with get, set

type [<AllowNullLiteral>] Stats =
    /// Array of channel statistics for each channel in the image.
    abstract channels: ResizeArray<ChannelStats> with get, set
    /// Value to identify if the image is opaque or transparent, based on the presence and use of alpha channel
    abstract isOpaque: bool with get, set
    /// Histogram-based estimation of greyscale entropy, discarding alpha channel if any (experimental)
    abstract entropy: float with get, set
    /// Estimation of greyscale sharpness based on the standard deviation of a Laplacian convolution, discarding alpha channel if any (experimental)
    abstract sharpness: float with get, set
    /// Object containing most dominant sRGB colour based on a 4096-bin 3D histogram (experimental)
    abstract dominant: StatsDominant with get, set

type [<AllowNullLiteral>] ChannelStats =
    /// minimum value in the channel
    abstract min: float with get, set
    /// maximum value in the channel
    abstract max: float with get, set
    /// sum of all values in a channel
    abstract sum: float with get, set
    /// sum of squared values in a channel
    abstract squaresSum: float with get, set
    /// mean of the values in a channel
    abstract mean: float with get, set
    /// standard deviation for the values in a channel
    abstract stdev: float with get, set
    /// x-coordinate of one of the pixel where the minimum lies
    abstract minX: float with get, set
    /// y-coordinate of one of the pixel where the minimum lies
    abstract minY: float with get, set
    /// x-coordinate of one of the pixel where the maximum lies
    abstract maxX: float with get, set
    /// y-coordinate of one of the pixel where the maximum lies
    abstract maxY: float with get, set

type [<AllowNullLiteral>] OutputOptions =
    /// Force format output, otherwise attempt to use input format (optional, default true)
    abstract force: bool option with get, set

type [<AllowNullLiteral>] JpegOptions =
    inherit OutputOptions
    /// Quality, integer 1-100 (optional, default 80)
    abstract quality: float option with get, set
    /// Use progressive (interlace) scan (optional, default false)
    abstract progressive: bool option with get, set
    /// Set to '4:4:4' to prevent chroma subsampling when quality <= 90 (optional, default '4:2:0')
    abstract chromaSubsampling: string option with get, set
    /// Apply trellis quantisation (optional, default  false)
    abstract trellisQuantisation: bool option with get, set
    /// Apply overshoot deringing (optional, default  false)
    abstract overshootDeringing: bool option with get, set
    /// Optimise progressive scans, forces progressive (optional, default false)
    abstract optimiseScans: bool option with get, set
    /// Alternative spelling of optimiseScans (optional, default false)
    abstract optimizeScans: bool option with get, set
    /// Optimise Huffman coding tables (optional, default true)
    abstract optimiseCoding: bool option with get, set
    /// Alternative spelling of optimiseCoding (optional, default true)
    abstract optimizeCoding: bool option with get, set
    /// Quantization table to use, integer 0-8 (optional, default 0)
    abstract quantisationTable: float option with get, set
    /// Alternative spelling of quantisationTable (optional, default 0)
    abstract quantizationTable: float option with get, set
    /// Use mozjpeg defaults (optional, default false)
    abstract mozjpeg: bool option with get, set

type [<AllowNullLiteral>] WebpOptions =
    inherit OutputOptions
    inherit AnimationOptions
    /// Quality, integer 1-100 (optional, default 80)
    abstract quality: float option with get, set
    /// Quality of alpha layer, number from 0-100 (optional, default 100)
    abstract alphaQuality: float option with get, set
    /// Use lossless compression mode (optional, default false)
    abstract lossless: bool option with get, set
    /// Use near_lossless compression mode (optional, default false)
    abstract nearLossless: bool option with get, set
    /// Use high quality chroma subsampling (optional, default false)
    abstract smartSubsample: bool option with get, set
    /// Level of CPU effort to reduce file size, integer 0-6 (optional, default 4)
    abstract reductionEffort: float option with get, set

type [<AllowNullLiteral>] AvifOptions =
    inherit OutputOptions
    /// quality, integer 1-100 (optional, default 50)
    abstract quality: float option with get, set
    /// use lossless compression (optional, default false)
    abstract lossless: bool option with get, set
    /// CPU effort vs file size, 0 (slowest/smallest) to 8 (fastest/largest) (optional, default 5)
    abstract speed: float option with get, set

type [<AllowNullLiteral>] HeifOptions =
    inherit OutputOptions
    /// quality, integer 1-100 (optional, default 50)
    abstract quality: float option with get, set
    /// compression format: av1, hevc (optional, default 'av1')
    abstract compression: HeifOptionsCompression option with get, set
    /// use lossless compression (optional, default false)
    abstract lossless: bool option with get, set
    /// CPU effort vs file size, 0 (slowest/smallest) to 8 (fastest/largest) (optional, default 5)
    abstract speed: float option with get, set

/// Requires libvips compiled with support for ImageMagick or GraphicsMagick.
/// The prebuilt binaries do not include this - see
/// {@link https://sharp.pixelplumbing.com/install#custom-libvips installing a custom libvips}.
type [<AllowNullLiteral>] GifOptions =
    inherit OutputOptions
    inherit AnimationOptions

type [<AllowNullLiteral>] TiffOptions =
    inherit OutputOptions
    /// Quality, integer 1-100 (optional, default 80)
    abstract quality: float option with get, set
    /// Compression options: lzw, deflate, jpeg, ccittfax4 (optional, default 'jpeg')
    abstract compression: string option with get, set
    /// Compression predictor options: none, horizontal, float (optional, default 'horizontal')
    abstract predictor: string option with get, set
    /// Write an image pyramid (optional, default false)
    abstract pyramid: bool option with get, set
    /// Write a tiled tiff (optional, default false)
    abstract tile: bool option with get, set
    /// Horizontal tile size (optional, default 256)
    abstract tileWidth: bool option with get, set
    /// Vertical tile size (optional, default 256)
    abstract tileHeight: bool option with get, set
    /// Horizontal resolution in pixels/mm (optional, default 1.0)
    abstract xres: float option with get, set
    /// Vertical resolution in pixels/mm (optional, default 1.0)
    abstract yres: float option with get, set
    /// Reduce bitdepth to 1, 2 or 4 bit (optional, default 8)
    abstract bitdepth: TiffOptionsBitdepth option with get, set

type [<AllowNullLiteral>] PngOptions =
    inherit OutputOptions
    /// Use progressive (interlace) scan (optional, default false)
    abstract progressive: bool option with get, set
    /// zlib compression level, 0-9 (optional, default 6)
    abstract compressionLevel: float option with get, set
    /// use adaptive row filtering (optional, default false)
    abstract adaptiveFiltering: bool option with get, set
    /// use the lowest number of colours needed to achieve given quality (optional, default `100`)
    abstract quality: float option with get, set
    /// Quantise to a palette-based image with alpha transparency support (optional, default false)
    abstract palette: bool option with get, set
    /// Maximum number of palette entries (optional, default 256)
    abstract colours: float option with get, set
    /// Alternative Spelling of "colours". Maximum number of palette entries (optional, default 256)
    abstract colors: float option with get, set
    /// Level of Floyd-Steinberg error diffusion (optional, default 1.0)
    abstract dither: float option with get, set

type [<AllowNullLiteral>] RotateOptions =
    /// parsed by the color module to extract values for red, green, blue and alpha. (optional, default "#000000")
    abstract background: Color option with get, set

type [<AllowNullLiteral>] FlattenOptions =
    /// background colour, parsed by the color module, defaults to black. (optional, default {r:0,g:0,b:0})
    abstract background: Color option with get, set

type [<AllowNullLiteral>] ResizeOptions =
    /// Alternative means of specifying width. If both are present this take priority.
    abstract width: float option with get, set
    /// Alternative means of specifying height. If both are present this take priority.
    abstract height: float option with get, set
    /// How the image should be resized to fit both provided dimensions, one of cover, contain, fill, inside or outside. (optional, default 'cover')
    abstract fit: FitEnum option with get, set
    /// Position, gravity or strategy to use when fit is cover or contain. (optional, default 'centre')
    abstract position: U2<float, string> option with get, set
    /// Background colour when using a fit of contain, parsed by the color module, defaults to black without transparency. (optional, default {r:0,g:0,b:0,alpha:1})
    abstract background: Color option with get, set
    /// The kernel to use for image reduction. (optional, default 'lanczos3')
    abstract kernel: KernelEnum option with get, set
    /// Do not enlarge if the width or height are already less than the specified dimensions, equivalent to GraphicsMagick's > geometry option. (optional, default false)
    abstract withoutEnlargement: bool option with get, set
    /// Take greater advantage of the JPEG and WebP shrink-on-load feature, which can lead to a slight moiré pattern on some images. (optional, default true)
    abstract fastShrinkOnLoad: bool option with get, set

type [<AllowNullLiteral>] Region =
    /// zero-indexed offset from left edge
    abstract left: float with get, set
    /// zero-indexed offset from top edge
    abstract top: float with get, set
    /// dimension of extracted image
    abstract width: float with get, set
    /// dimension of extracted image
    abstract height: float with get, set

type [<AllowNullLiteral>] ExtendOptions =
    /// single pixel count to top edge (optional, default 0)
    abstract top: float option with get, set
    /// single pixel count to left edge (optional, default 0)
    abstract left: float option with get, set
    /// single pixel count to bottom edge (optional, default 0)
    abstract bottom: float option with get, set
    /// single pixel count to right edge (optional, default 0)
    abstract right: float option with get, set
    /// background colour, parsed by the color module, defaults to black without transparency. (optional, default {r:0,g:0,b:0,alpha:1})
    abstract background: Color option with get, set

type [<RequireQualifiedAccess>] Channels =
    | N3 = 3
    | N4 = 4

type [<AllowNullLiteral>] RGBA =
    abstract r: float option with get, set
    abstract g: float option with get, set
    abstract b: float option with get, set
    abstract alpha: float option with get, set

type Color =
    U2<string, RGBA>

type [<AllowNullLiteral>] Kernel =
    /// width of the kernel in pixels.
    abstract width: float with get, set
    /// height of the kernel in pixels.
    abstract height: float with get, set
    /// Array of length width*height containing the kernel values.
    abstract kernel: ArrayLike<float> with get, set
    /// the scale of the kernel in pixels. (optional, default sum)
    abstract scale: float option with get, set
    /// the offset of the kernel in pixels. (optional, default 0)
    abstract offset: float option with get, set

type [<AllowNullLiteral>] ClaheOptions =
    /// width of the region
    abstract width: float with get, set
    /// height of the region
    abstract height: float with get, set
    /// max slope of the cumulative contrast. (optional, default 3)
    abstract maxSlope: float option with get, set

type [<AllowNullLiteral>] ThresholdOptions =
    /// convert to single channel greyscale. (optional, default true)
    abstract greyscale: bool option with get, set
    /// alternative spelling for greyscale. (optional, default true)
    abstract grayscale: bool option with get, set

type [<AllowNullLiteral>] OverlayOptions =
    /// Buffer containing image data, String containing the path to an image file, or Create object
    abstract input: U3<string, Buffer, OverlayOptionsInput> option with get, set
    /// how to blend this image with the image below. (optional, default `'over'`)
    abstract blend: Blend option with get, set
    /// gravity at which to place the overlay. (optional, default 'centre')
    abstract gravity: Gravity option with get, set
    /// the pixel offset from the top edge.
    abstract top: float option with get, set
    /// the pixel offset from the left edge.
    abstract left: float option with get, set
    /// set to true to repeat the overlay image across the entire image with the given  gravity. (optional, default false)
    abstract tile: bool option with get, set
    /// number representing the DPI for vector overlay image. (optional, default 72)
    abstract density: float option with get, set
    /// describes overlay when using raw pixel data.
    abstract raw: Raw option with get, set
    /// Set to true to avoid premultipling the image below. Equivalent to the --premultiplied vips option.
    abstract premultiplied: bool option with get, set
    /// Do not process input images where the number of pixels (width x height) exceeds this limit.
    /// Assumes image dimensions contained in the input metadata can be trusted.
    /// An integral Number of pixels, zero or false to remove limit, true to use default limit of 268402689 (0x3FFF x 0x3FFF). (optional, default 268402689)
    abstract limitInputPixels: U2<float, bool> option with get, set

type [<AllowNullLiteral>] TileOptions =
    /// Tile size in pixels, a value between 1 and 8192. (optional, default 256)
    abstract size: float option with get, set
    /// Tile overlap in pixels, a value between 0 and 8192. (optional, default 0)
    abstract overlap: float option with get, set
    /// Tile angle of rotation, must be a multiple of 90. (optional, default 0)
    abstract angle: float option with get, set
    /// background colour, parsed by the color module, defaults to white without transparency. (optional, default {r:255,g:255,b:255,alpha:1})
    abstract background: U2<string, RGBA> option with get, set
    /// How deep to make the pyramid, possible values are "onepixel", "onetile" or "one" (default based on layout)
    abstract depth: string option with get, set
    /// Threshold to skip tile generation, a value 0 - 255 for 8-bit images or 0 - 65535 for 16-bit images
    abstract skipBlanks: float option with get, set
    /// Tile container, with value fs (filesystem) or zip (compressed file). (optional, default 'fs')
    abstract container: string option with get, set
    /// Filesystem layout, possible values are dz, iiif, zoomify or google. (optional, default 'dz')
    abstract layout: TileLayout option with get, set

type [<AllowNullLiteral>] AnimationOptions =
    /// Page height for animated output, a value greater than 0. (optional)
    abstract pageHeight: float option with get, set
    /// Number of animation iterations, a value between 0 and 65535. Use 0 for infinite animation. (optional, default 0)
    abstract loop: float option with get, set
    /// List of delays between animation frames (in milliseconds), each value between 0 and 65535. (optional)
    abstract delay: ResizeArray<float> option with get, set

type [<AllowNullLiteral>] OutputInfo =
    abstract format: string with get, set
    abstract size: float with get, set
    abstract width: float with get, set
    abstract height: float with get, set
    abstract channels: float with get, set
    /// indicating if premultiplication was used
    abstract premultiplied: bool with get, set
    /// Only defined when using a crop strategy
    abstract cropOffsetLeft: float option with get, set
    /// Only defined when using a crop strategy
    abstract cropOffsetTop: float option with get, set
    /// Only defined when using a trim method
    abstract trimOffsetLeft: float option with get, set
    /// Only defined when using a trim method
    abstract trimOffsetTop: float option with get, set

type [<AllowNullLiteral>] AvailableFormatInfo =
    abstract id: string with get, set
    abstract input: AvailableFormatInfoInput with get, set
    abstract output: AvailableFormatInfoInput with get, set

type [<AllowNullLiteral>] FitEnum =
    abstract contain: string with get, set
    abstract cover: string with get, set
    abstract fill: string with get, set
    abstract inside: string with get, set
    abstract outside: string with get, set

type [<AllowNullLiteral>] KernelEnum =
    abstract nearest: string with get, set
    abstract cubic: string with get, set
    abstract mitchell: string with get, set
    abstract lanczos2: string with get, set
    abstract lanczos3: string with get, set

type [<AllowNullLiteral>] BoolEnum =
    abstract ``and``: string with get, set
    abstract ``or``: string with get, set
    abstract eor: string with get, set

type [<AllowNullLiteral>] ColourspaceEnum =
    abstract multiband: string with get, set
    abstract ``b-w``: string with get, set
    abstract bw: string with get, set
    abstract cmyk: string with get, set
    abstract srgb: string with get, set

type [<StringEnum>] [<RequireQualifiedAccess>] TileLayout =
    | Dz
    | Iiif
    | Zoomify
    | Google

type [<StringEnum>] [<RequireQualifiedAccess>] Blend =
    | Clear
    | Source
    | Over
    | In
    | Out
    | Atop
    | Dest
    | [<CompiledName "dest-over">] DestOver
    | [<CompiledName "dest-in">] DestIn
    | [<CompiledName "dest-out">] DestOut
    | [<CompiledName "dest-atop">] DestAtop
    | Xor
    | Add
    | Saturate
    | Multiply
    | Screen
    | Overlay
    | Darken
    | Lighten
    | [<CompiledName "colour-dodge">] ColourDodge
    | [<CompiledName "colour-burn">] ColourBurn
    | [<CompiledName "hard-light">] HardLight
    | [<CompiledName "soft-light">] SoftLight
    | Difference
    | Exclusion

type Gravity =
    U2<float, string>

type [<AllowNullLiteral>] GravityEnum =
    abstract north: float with get, set
    abstract northeast: float with get, set
    abstract southeast: float with get, set
    abstract south: float with get, set
    abstract southwest: float with get, set
    abstract west: float with get, set
    abstract northwest: float with get, set
    abstract east: float with get, set
    abstract center: float with get, set
    abstract centre: float with get, set

type [<AllowNullLiteral>] StrategyEnum =
    abstract entropy: float with get, set
    abstract attention: float with get, set

type [<AllowNullLiteral>] FormatEnum =
    abstract avif: AvailableFormatInfo with get, set
    abstract dz: AvailableFormatInfo with get, set
    abstract fits: AvailableFormatInfo with get, set
    abstract gif: AvailableFormatInfo with get, set
    abstract heif: AvailableFormatInfo with get, set
    abstract input: AvailableFormatInfo with get, set
    abstract jpeg: AvailableFormatInfo with get, set
    abstract jpg: AvailableFormatInfo with get, set
    abstract magick: AvailableFormatInfo with get, set
    abstract openslide: AvailableFormatInfo with get, set
    abstract pdf: AvailableFormatInfo with get, set
    abstract png: AvailableFormatInfo with get, set
    abstract ppm: AvailableFormatInfo with get, set
    abstract raw: AvailableFormatInfo with get, set
    abstract svg: AvailableFormatInfo with get, set
    abstract tiff: AvailableFormatInfo with get, set
    abstract v: AvailableFormatInfo with get, set
    abstract webp: AvailableFormatInfo with get, set

type [<AllowNullLiteral>] CacheResult =
    abstract memory: CacheResultMemory with get, set
    abstract files: CacheResultFiles with get, set
    abstract items: CacheResultFiles with get, set

type Matrix3x3 =
    float * float * float * float * float * float * float * float * float

type [<AllowNullLiteral>] IExportsVersions =
    abstract vips: string with get, set
    abstract cairo: string option with get, set
    abstract croco: string option with get, set
    abstract exif: string option with get, set
    abstract expat: string option with get, set
    abstract ffi: string option with get, set
    abstract fontconfig: string option with get, set
    abstract freetype: string option with get, set
    abstract gdkpixbuf: string option with get, set
    abstract gif: string option with get, set
    abstract glib: string option with get, set
    abstract gsf: string option with get, set
    abstract harfbuzz: string option with get, set
    abstract jpeg: string option with get, set
    abstract lcms: string option with get, set
    abstract orc: string option with get, set
    abstract pango: string option with get, set
    abstract pixman: string option with get, set
    abstract png: string option with get, set
    abstract svg: string option with get, set
    abstract tiff: string option with get, set
    abstract webp: string option with get, set
    abstract avif: string option with get, set
    abstract heif: string option with get, set
    abstract xml: string option with get, set
    abstract zlib: string option with get, set

type [<AllowNullLiteral>] SharpToBufferPromise =
    abstract data: Buffer with get, set
    abstract info: OutputInfo with get, set

type [<RequireQualifiedAccess>] RawChannels =
    | N1 = 1
    | N2 = 2
    | N3 = 3
    | N4 = 4

type [<AllowNullLiteral>] StatsDominant =
    abstract r: float with get, set
    abstract g: float with get, set
    abstract b: float with get, set

type [<StringEnum>] [<RequireQualifiedAccess>] HeifOptionsCompression =
    | Av1
    | Hevc

type [<RequireQualifiedAccess>] TiffOptionsBitdepth =
    | N1 = 1
    | N2 = 2
    | N4 = 4
    | N8 = 8

type [<AllowNullLiteral>] OverlayOptionsInput =
    abstract create: Create with get, set

type [<AllowNullLiteral>] AvailableFormatInfoInput =
    abstract file: bool with get, set
    abstract buffer: bool with get, set
    abstract stream: bool with get, set

type [<AllowNullLiteral>] CacheResultMemory =
    abstract current: float with get, set
    abstract high: float with get, set
    abstract max: float with get, set

type [<AllowNullLiteral>] CacheResultFiles =
    abstract current: float with get, set
    abstract max: float with get, set
