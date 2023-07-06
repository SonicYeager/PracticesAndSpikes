using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotChocolate.Checker.Migrations
{
    /// <inheritdoc />
    public partial class Inital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SurName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEntity", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BookEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    PublicationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ISBN = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Genre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PageCount = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookEntity_UserEntity_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "UserEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "UserEntity",
                columns: new[] { "Id", "Name", "SurName" },
                values: new object[,]
                {
                    { 1, "Janis", "Hermann" },
                    { 2, "Tanya", "Hartmann" },
                    { 3, "Kenny", "Okuneva" },
                    { 4, "Ken", "Koch" },
                    { 5, "Kerry", "Brekke" },
                    { 6, "Ida", "Torp" },
                    { 7, "Candace", "Heller" },
                    { 8, "Veronica", "Schmitt" },
                    { 9, "Debbie", "Kessler" },
                    { 10, "Wallace", "Deckow" },
                    { 11, "Kimberly", "Wiza" },
                    { 12, "Cesar", "Schinner" },
                    { 13, "Ebony", "Gislason" },
                    { 14, "Mindy", "Bergstrom" },
                    { 15, "Carrie", "Gorczany" },
                    { 16, "Maxine", "Kshlerin" },
                    { 17, "Crystal", "Durgan" },
                    { 18, "Denise", "Waters" },
                    { 19, "Roger", "Simonis" },
                    { 20, "Nina", "Littel" },
                    { 21, "Corey", "Schultz" },
                    { 22, "Tim", "Fay" },
                    { 23, "Dave", "O'Reilly" },
                    { 24, "Matt", "Spinka" },
                    { 25, "Jean", "Kunze" },
                    { 26, "Loren", "Crist" },
                    { 27, "Stephanie", "Armstrong" },
                    { 28, "Timmy", "Halvorson" },
                    { 29, "Tami", "Reilly" },
                    { 30, "Lynn", "Keebler" },
                    { 31, "Lindsey", "Toy" },
                    { 32, "Maxine", "Hermiston" },
                    { 33, "Jonathan", "Jacobs" },
                    { 34, "Enrique", "Bartoletti" },
                    { 35, "Ismael", "Herzog" },
                    { 36, "Donald", "Hartmann" },
                    { 37, "Erma", "Hane" },
                    { 38, "Karla", "Kutch" },
                    { 39, "Matt", "Mertz" },
                    { 40, "Lewis", "Schiller" },
                    { 41, "Lamar", "Koch" },
                    { 42, "Nora", "Lindgren" },
                    { 43, "Tammy", "Graham" },
                    { 44, "Jeannie", "Baumbach" },
                    { 45, "Peter", "Ledner" },
                    { 46, "Don", "Waelchi" },
                    { 47, "Jamie", "Turcotte" },
                    { 48, "Derrick", "Jenkins" },
                    { 49, "Freddie", "Adams" },
                    { 50, "Kristen", "Roob" },
                    { 51, "Becky", "Stamm" },
                    { 52, "Frank", "Johnston" },
                    { 53, "Joy", "Beahan" },
                    { 54, "Eduardo", "Nitzsche" },
                    { 55, "Jason", "Predovic" },
                    { 56, "Rufus", "Bradtke" },
                    { 57, "Lawrence", "Hauck" },
                    { 58, "Olivia", "Bruen" },
                    { 59, "Cedric", "Yost" },
                    { 60, "Jimmie", "Vandervort" },
                    { 61, "Camille", "Bergnaum" },
                    { 62, "Lee", "Kris" },
                    { 63, "Diane", "Witting" },
                    { 64, "Daisy", "Graham" },
                    { 65, "Wallace", "Rohan" },
                    { 66, "Mandy", "Powlowski" },
                    { 67, "Jody", "Stehr" },
                    { 68, "Carlton", "Kutch" },
                    { 69, "Jennie", "Miller" },
                    { 70, "Jessie", "Powlowski" },
                    { 71, "Domingo", "Raynor" },
                    { 72, "Johnnie", "O'Conner" },
                    { 73, "Tracy", "Beer" },
                    { 74, "Nellie", "Sawayn" },
                    { 75, "Ernest", "Grady" },
                    { 76, "Della", "Jones" },
                    { 77, "Constance", "Flatley" },
                    { 78, "Terry", "Greenfelder" },
                    { 79, "Bertha", "Schneider" },
                    { 80, "Tracy", "Wolf" },
                    { 81, "Della", "Douglas" },
                    { 82, "Jesse", "Dicki" },
                    { 83, "Greg", "Greenholt" },
                    { 84, "Hattie", "Kiehn" },
                    { 85, "Levi", "Beier" },
                    { 86, "Leslie", "Pagac" },
                    { 87, "Lawrence", "Bailey" },
                    { 88, "Bernadette", "Olson" },
                    { 89, "Sheri", "Heller" },
                    { 90, "Wanda", "Tillman" },
                    { 91, "Suzanne", "Kulas" },
                    { 92, "Claire", "Lehner" },
                    { 93, "Shawn", "Kerluke" },
                    { 94, "Norma", "Witting" },
                    { 95, "Lauren", "Murazik" },
                    { 96, "Elaine", "Adams" },
                    { 97, "Nichole", "Bruen" },
                    { 98, "Jeanette", "Corwin" },
                    { 99, "Marshall", "Abshire" }
                });

            migrationBuilder.InsertData(
                table: "BookEntity",
                columns: new[] { "Id", "AuthorId", "Genre", "ISBN", "Language", "PageCount", "PublicationDate", "Title" },
                values: new object[,]
                {
                    { 1, 1, "Latin", "2578066949413", "Research", 8461, new DateTime(2022, 10, 22, 5, 50, 9, 98, DateTimeKind.Local).AddTicks(5870), "Koch Inc" },
                    { 2, 2, "Blues", "6080081688483", "Wisconsin", 1678, new DateTime(2022, 12, 7, 16, 50, 27, 677, DateTimeKind.Local).AddTicks(7000), "Armstrong - Wolf" },
                    { 3, 3, "Funk", "2198401616918", "back-end", 278, new DateTime(2022, 7, 27, 7, 0, 53, 163, DateTimeKind.Local).AddTicks(9715), "Sanford, Marquardt and Buckridge" },
                    { 4, 4, "Pop", "9709776571240", "generate", 1360, new DateTime(2023, 6, 23, 22, 46, 44, 159, DateTimeKind.Local).AddTicks(5832), "Feil LLC" },
                    { 5, 5, "Soul", "0706326643951", "Grass-roots", 7214, new DateTime(2022, 7, 12, 9, 27, 35, 728, DateTimeKind.Local).AddTicks(9573), "Lowe - Mann" },
                    { 6, 6, "Stage And Screen", "8874111695644", "Mandatory", 8879, new DateTime(2023, 3, 4, 12, 54, 13, 562, DateTimeKind.Local).AddTicks(7074), "Ullrich Inc" },
                    { 7, 7, "Rap", "3579108222142", "initiative", 5361, new DateTime(2023, 7, 4, 23, 18, 38, 552, DateTimeKind.Local).AddTicks(2257), "VonRueden Group" },
                    { 8, 8, "Rock", "7557348716566", "Vatu", 2704, new DateTime(2023, 3, 7, 1, 22, 18, 597, DateTimeKind.Local).AddTicks(7827), "Wolf, Crona and Dickens" },
                    { 9, 9, "Non Music", "9914130873537", "Identity", 1881, new DateTime(2022, 8, 28, 4, 35, 44, 22, DateTimeKind.Local).AddTicks(5784), "Block and Sons" },
                    { 10, 10, "Blues", "2122528039480", "methodologies", 1350, new DateTime(2022, 12, 27, 23, 27, 55, 60, DateTimeKind.Local).AddTicks(8535), "Mueller LLC" },
                    { 11, 11, "Pop", "5850767118907", "Handmade Wooden Towels", 7336, new DateTime(2022, 10, 26, 22, 57, 8, 146, DateTimeKind.Local).AddTicks(1247), "Bernhard, Ledner and Breitenberg" },
                    { 12, 12, "Hip Hop", "7683554599805", "British Indian Ocean Territory (Chagos Archipelago)", 9709, new DateTime(2023, 1, 6, 22, 48, 15, 511, DateTimeKind.Local).AddTicks(9050), "Borer - Mayert" },
                    { 13, 13, "Country", "0939926533128", "Trail", 5923, new DateTime(2023, 6, 5, 8, 41, 4, 578, DateTimeKind.Local).AddTicks(7377), "Simonis - Dickinson" },
                    { 14, 14, "Non Music", "9803219960643", "Unbranded Frozen Hat", 7260, new DateTime(2022, 11, 12, 8, 45, 49, 427, DateTimeKind.Local).AddTicks(2315), "Wilkinson, Dicki and Blick" },
                    { 15, 15, "Jazz", "0795044075762", "bypassing", 3498, new DateTime(2022, 9, 24, 3, 51, 50, 38, DateTimeKind.Local).AddTicks(2694), "Herman - MacGyver" },
                    { 16, 16, "Non Music", "8808016471928", "sexy", 3660, new DateTime(2023, 1, 17, 7, 41, 6, 800, DateTimeKind.Local).AddTicks(7912), "Parisian Inc" },
                    { 17, 17, "Non Music", "9156355425430", "Customer", 2037, new DateTime(2022, 8, 23, 22, 48, 36, 335, DateTimeKind.Local).AddTicks(9384), "Welch LLC" },
                    { 18, 18, "Metal", "8755236184834", "Handcrafted Frozen Gloves", 1480, new DateTime(2023, 3, 11, 7, 48, 38, 727, DateTimeKind.Local).AddTicks(3702), "Aufderhar - Erdman" },
                    { 19, 19, "Funk", "9688358714766", "grey", 2076, new DateTime(2022, 7, 23, 13, 58, 42, 459, DateTimeKind.Local).AddTicks(778), "Metz Group" },
                    { 20, 20, "Rock", "5648646388356", "Canada", 6547, new DateTime(2023, 5, 1, 12, 11, 8, 70, DateTimeKind.Local).AddTicks(8560), "Emmerich LLC" },
                    { 21, 21, "Stage And Screen", "8706719727499", "e-enable", 3277, new DateTime(2022, 8, 21, 0, 42, 24, 962, DateTimeKind.Local).AddTicks(6994), "Hackett - Gleason" },
                    { 22, 22, "World", "1093955065902", "Chief", 8551, new DateTime(2023, 2, 7, 0, 7, 35, 991, DateTimeKind.Local).AddTicks(4652), "Lind - Ward" },
                    { 23, 23, "Folk", "4178294105643", "calculate", 8112, new DateTime(2022, 12, 31, 17, 2, 52, 806, DateTimeKind.Local).AddTicks(1927), "Murphy - Beahan" },
                    { 24, 24, "Rap", "1521207064989", "Saint Kitts and Nevis", 6662, new DateTime(2023, 7, 2, 3, 40, 58, 391, DateTimeKind.Local).AddTicks(8509), "Prohaska, Murray and Nienow" },
                    { 25, 25, "Pop", "7092145901611", "hack", 6825, new DateTime(2022, 7, 18, 6, 23, 47, 483, DateTimeKind.Local).AddTicks(9180), "Botsford - Will" },
                    { 26, 26, "Country", "6162601546425", "integrate", 7293, new DateTime(2022, 11, 11, 19, 43, 0, 184, DateTimeKind.Local).AddTicks(4474), "Kutch and Sons" },
                    { 27, 27, "Funk", "3526215834225", "Mobility", 3571, new DateTime(2023, 3, 27, 8, 1, 30, 262, DateTimeKind.Local).AddTicks(8732), "Fisher and Sons" },
                    { 28, 28, "Latin", "5412671905280", "Peso Uruguayo", 7602, new DateTime(2023, 4, 9, 1, 37, 14, 679, DateTimeKind.Local).AddTicks(6298), "Swaniawski - Leffler" },
                    { 29, 29, "Country", "1527854874377", "Electronics", 2221, new DateTime(2023, 6, 3, 19, 2, 36, 779, DateTimeKind.Local).AddTicks(1553), "Koch - DuBuque" },
                    { 30, 30, "Hip Hop", "2301627798003", "Diverse", 3444, new DateTime(2023, 1, 20, 10, 18, 43, 280, DateTimeKind.Local).AddTicks(9874), "Larson - Feil" },
                    { 31, 31, "Rock", "6073973844067", "hierarchy", 8458, new DateTime(2023, 2, 25, 22, 47, 34, 345, DateTimeKind.Local).AddTicks(2465), "Torphy and Sons" },
                    { 32, 32, "Rock", "7202473762338", "back up", 5238, new DateTime(2022, 11, 28, 1, 12, 11, 126, DateTimeKind.Local).AddTicks(3484), "Jenkins, Cartwright and Wiegand" },
                    { 33, 33, "Jazz", "5306490763704", "Generic Wooden Bacon", 3909, new DateTime(2023, 5, 1, 6, 12, 13, 848, DateTimeKind.Local).AddTicks(4824), "Spinka - Dare" },
                    { 34, 34, "Funk", "9900681554810", "Serbian Dinar", 8663, new DateTime(2023, 2, 8, 9, 7, 47, 360, DateTimeKind.Local).AddTicks(8063), "Altenwerth and Sons" },
                    { 35, 35, "Electronic", "3799817989154", "Texas", 801, new DateTime(2022, 10, 13, 11, 42, 42, 24, DateTimeKind.Local).AddTicks(4342), "Schmitt Group" },
                    { 36, 36, "Non Music", "6409724237121", "back up", 9029, new DateTime(2023, 6, 26, 23, 9, 50, 565, DateTimeKind.Local).AddTicks(694), "Roberts Group" },
                    { 37, 37, "Folk", "5440064373330", "Assistant", 873, new DateTime(2023, 3, 24, 3, 10, 56, 93, DateTimeKind.Local).AddTicks(726), "Bartoletti - Will" },
                    { 38, 38, "Country", "5873924663416", "Money Market Account", 2248, new DateTime(2023, 3, 12, 11, 43, 29, 798, DateTimeKind.Local).AddTicks(4043), "Reilly, Dibbert and Wuckert" },
                    { 39, 39, "Country", "5388238303922", "Jewelery & Books", 3259, new DateTime(2022, 10, 24, 10, 4, 1, 77, DateTimeKind.Local).AddTicks(8043), "Brown, Langosh and Gorczany" },
                    { 40, 40, "Blues", "4574136044734", "Wall", 871, new DateTime(2023, 4, 11, 10, 34, 59, 497, DateTimeKind.Local).AddTicks(129), "Rogahn, Williamson and VonRueden" },
                    { 41, 41, "Blues", "7181682455227", "pixel", 5181, new DateTime(2023, 5, 5, 3, 19, 17, 264, DateTimeKind.Local).AddTicks(8777), "Parisian, Reichel and Schmidt" },
                    { 42, 42, "World", "1791282010588", "Research", 9682, new DateTime(2023, 4, 14, 18, 32, 11, 837, DateTimeKind.Local).AddTicks(9325), "Kautzer - Doyle" },
                    { 43, 43, "Reggae", "2162881148870", "Saint Helena Pound", 2811, new DateTime(2022, 7, 16, 22, 16, 2, 435, DateTimeKind.Local).AddTicks(1554), "Hartmann, Goldner and Labadie" },
                    { 44, 44, "Electronic", "2982568282020", "Sleek Wooden Gloves", 2323, new DateTime(2023, 7, 6, 18, 22, 15, 485, DateTimeKind.Local).AddTicks(7910), "Hamill Inc" },
                    { 45, 45, "Classical", "1346460913870", "Business-focused", 4345, new DateTime(2022, 11, 21, 5, 11, 17, 266, DateTimeKind.Local).AddTicks(4086), "Dooley, Kuhn and Schimmel" },
                    { 46, 46, "Stage And Screen", "8073488447478", "neural", 1364, new DateTime(2023, 7, 2, 1, 59, 45, 905, DateTimeKind.Local).AddTicks(871), "Mayer - Daniel" },
                    { 47, 47, "World", "6021484205129", "help-desk", 8803, new DateTime(2022, 8, 24, 6, 31, 7, 159, DateTimeKind.Local).AddTicks(1179), "Denesik LLC" },
                    { 48, 48, "Blues", "4840332721718", "deposit", 27, new DateTime(2023, 2, 13, 15, 43, 14, 47, DateTimeKind.Local).AddTicks(478), "Ratke LLC" },
                    { 49, 49, "World", "4995979972075", "Licensed", 4130, new DateTime(2022, 7, 29, 19, 27, 10, 907, DateTimeKind.Local).AddTicks(7282), "Hermann, Murphy and Stiedemann" },
                    { 50, 50, "Rap", "3383695458828", "synthesizing", 7652, new DateTime(2023, 3, 14, 20, 37, 24, 359, DateTimeKind.Local).AddTicks(2720), "Emmerich - Kunde" },
                    { 51, 51, "Jazz", "8707165386834", "overriding", 8154, new DateTime(2022, 10, 8, 21, 13, 5, 646, DateTimeKind.Local).AddTicks(7684), "Bahringer Inc" },
                    { 52, 52, "Reggae", "4027091618835", "tan", 215, new DateTime(2022, 11, 16, 7, 31, 58, 871, DateTimeKind.Local).AddTicks(4347), "Strosin - Leffler" },
                    { 53, 53, "Blues", "3845576527387", "Versatile", 8869, new DateTime(2022, 12, 20, 16, 13, 11, 941, DateTimeKind.Local).AddTicks(2188), "Lebsack - Torp" },
                    { 54, 54, "Country", "1377072073928", "Japan", 4842, new DateTime(2022, 7, 28, 10, 29, 38, 862, DateTimeKind.Local).AddTicks(9574), "Monahan - O'Connell" },
                    { 55, 55, "Metal", "3202417408692", "Licensed Wooden Towels", 3599, new DateTime(2022, 7, 23, 1, 17, 44, 197, DateTimeKind.Local).AddTicks(8732), "Green Group" },
                    { 56, 56, "Classical", "6090464601596", "Afghanistan", 8511, new DateTime(2022, 11, 2, 12, 24, 36, 46, DateTimeKind.Local).AddTicks(7375), "Kassulke - Green" },
                    { 57, 57, "Stage And Screen", "0247948950992", "Savings Account", 4008, new DateTime(2022, 12, 8, 11, 54, 21, 874, DateTimeKind.Local).AddTicks(8997), "Boyle and Sons" },
                    { 58, 58, "Folk", "5762760748503", "calculate", 6514, new DateTime(2022, 9, 27, 15, 28, 39, 986, DateTimeKind.Local).AddTicks(6813), "Sporer, Wolf and Gulgowski" },
                    { 59, 59, "Classical", "2100442614884", "Ergonomic", 2687, new DateTime(2023, 2, 14, 11, 35, 12, 455, DateTimeKind.Local).AddTicks(9531), "Watsica - Kozey" },
                    { 60, 60, "Pop", "6482218023851", "Awesome Plastic Computer", 7130, new DateTime(2022, 10, 10, 4, 14, 2, 272, DateTimeKind.Local).AddTicks(6585), "Balistreri Group" },
                    { 61, 61, "Metal", "2115697920369", "methodology", 1472, new DateTime(2023, 6, 20, 4, 3, 41, 434, DateTimeKind.Local).AddTicks(2634), "Stracke, Bauch and Cruickshank" },
                    { 62, 62, "Country", "7543393522351", "Cambridgeshire", 3053, new DateTime(2023, 6, 20, 3, 38, 25, 858, DateTimeKind.Local).AddTicks(5257), "Carroll LLC" },
                    { 63, 63, "Rap", "2690825257758", "SDR", 893, new DateTime(2022, 9, 27, 8, 38, 50, 243, DateTimeKind.Local).AddTicks(8376), "Heidenreich, Oberbrunner and Nicolas" },
                    { 64, 64, "Stage And Screen", "2686358190402", "Clothing & Sports", 4638, new DateTime(2022, 11, 8, 15, 43, 12, 369, DateTimeKind.Local).AddTicks(6876), "Graham, Schmeler and Renner" },
                    { 65, 65, "Stage And Screen", "9874281405486", "Delaware", 8357, new DateTime(2023, 1, 10, 21, 33, 44, 685, DateTimeKind.Local).AddTicks(1576), "Blanda, Bode and Ullrich" },
                    { 66, 66, "Funk", "4875435097491", "South Carolina", 5878, new DateTime(2022, 7, 14, 21, 8, 21, 901, DateTimeKind.Local).AddTicks(2266), "Trantow, White and Emmerich" },
                    { 67, 67, "Blues", "9773514420220", "Sleek", 687, new DateTime(2022, 11, 24, 19, 22, 32, 410, DateTimeKind.Local).AddTicks(233), "Sawayn - Wuckert" },
                    { 68, 68, "Electronic", "5080240986062", "lime", 3681, new DateTime(2022, 11, 4, 15, 5, 57, 906, DateTimeKind.Local).AddTicks(8061), "Hintz LLC" },
                    { 69, 69, "Blues", "4740892936402", "Security", 7727, new DateTime(2022, 10, 5, 15, 54, 56, 698, DateTimeKind.Local).AddTicks(1357), "Metz LLC" },
                    { 70, 70, "Soul", "1149036496353", "withdrawal", 1585, new DateTime(2023, 4, 16, 22, 39, 34, 211, DateTimeKind.Local).AddTicks(9390), "Cronin Group" },
                    { 71, 71, "Classical", "0113923503081", "Innovative", 3025, new DateTime(2022, 12, 21, 12, 48, 37, 445, DateTimeKind.Local).AddTicks(4402), "Hills Group" },
                    { 72, 72, "Electronic", "4576473011493", "communities", 2537, new DateTime(2022, 11, 1, 21, 48, 10, 982, DateTimeKind.Local).AddTicks(8708), "Jakubowski and Sons" },
                    { 73, 73, "Rap", "8881564747783", "USB", 4654, new DateTime(2022, 10, 4, 2, 8, 24, 243, DateTimeKind.Local).AddTicks(4162), "West, Gutkowski and Mante" },
                    { 74, 74, "Folk", "9139825520484", "National", 735, new DateTime(2023, 5, 25, 20, 53, 53, 679, DateTimeKind.Local).AddTicks(511), "Grant and Sons" },
                    { 75, 75, "Blues", "2195335476069", "Pataca", 1500, new DateTime(2022, 9, 3, 0, 20, 32, 213, DateTimeKind.Local).AddTicks(916), "Rogahn - Schaefer" },
                    { 76, 76, "World", "8448597927485", "Interactions", 3459, new DateTime(2023, 1, 30, 6, 52, 3, 9, DateTimeKind.Local).AddTicks(7469), "Treutel - Strosin" },
                    { 77, 77, "Electronic", "2228869250499", "metrics", 6657, new DateTime(2022, 12, 6, 0, 23, 56, 663, DateTimeKind.Local).AddTicks(9749), "Schaden, Monahan and Predovic" },
                    { 78, 78, "Soul", "4566293708857", "panel", 1803, new DateTime(2022, 9, 14, 8, 14, 17, 877, DateTimeKind.Local).AddTicks(702), "Zemlak - Kuvalis" },
                    { 79, 79, "Classical", "8529985825443", "Bahrain", 3737, new DateTime(2022, 9, 11, 9, 1, 1, 746, DateTimeKind.Local).AddTicks(761), "Simonis, Effertz and Koss" },
                    { 80, 80, "Hip Hop", "7948214038859", "Lead", 2298, new DateTime(2023, 7, 4, 2, 31, 2, 97, DateTimeKind.Local).AddTicks(9663), "Beier Inc" },
                    { 81, 81, "Non Music", "8728938629330", "framework", 2847, new DateTime(2022, 8, 7, 16, 52, 29, 153, DateTimeKind.Local).AddTicks(5675), "Ruecker - Kris" },
                    { 82, 82, "Jazz", "0053848008141", "Missouri", 5998, new DateTime(2023, 1, 24, 1, 11, 40, 551, DateTimeKind.Local).AddTicks(6948), "D'Amore, Moen and Simonis" },
                    { 83, 83, "Jazz", "2418523165744", "indexing", 2172, new DateTime(2023, 1, 25, 12, 53, 47, 383, DateTimeKind.Local).AddTicks(8139), "McKenzie, Zulauf and Zemlak" },
                    { 84, 84, "Classical", "5385929872606", "Rial Omani", 3083, new DateTime(2022, 12, 7, 11, 6, 10, 264, DateTimeKind.Local).AddTicks(8020), "Vandervort, Predovic and Stracke" },
                    { 85, 85, "World", "4701884611424", "Handmade Steel Computer", 4773, new DateTime(2023, 4, 26, 23, 9, 21, 652, DateTimeKind.Local).AddTicks(1641), "Ryan - Ziemann" },
                    { 86, 86, "Funk", "5293177636119", "matrix", 5871, new DateTime(2022, 11, 19, 3, 29, 0, 436, DateTimeKind.Local).AddTicks(5972), "Hand Inc" },
                    { 87, 87, "Blues", "2218983210868", "regional", 4278, new DateTime(2022, 9, 26, 2, 59, 30, 965, DateTimeKind.Local).AddTicks(4885), "Schmidt - Dare" },
                    { 88, 88, "Rap", "3347637828636", "Gorgeous Rubber Car", 7019, new DateTime(2023, 4, 23, 9, 32, 25, 829, DateTimeKind.Local).AddTicks(6492), "Hammes - Boehm" },
                    { 89, 89, "Reggae", "0309745353328", "cohesive", 3585, new DateTime(2023, 5, 23, 4, 3, 54, 643, DateTimeKind.Local).AddTicks(7537), "Stoltenberg, Kuhic and Ullrich" },
                    { 90, 90, "Hip Hop", "6002379349050", "Unbranded Fresh Car", 5630, new DateTime(2023, 6, 1, 20, 18, 41, 345, DateTimeKind.Local).AddTicks(4264), "Conn, Lemke and Keebler" },
                    { 91, 91, "Electronic", "4527669005369", "Program", 2903, new DateTime(2023, 4, 3, 5, 24, 34, 569, DateTimeKind.Local).AddTicks(8138), "Hammes and Sons" },
                    { 92, 92, "Jazz", "3879695395251", "program", 2849, new DateTime(2022, 10, 8, 3, 21, 28, 314, DateTimeKind.Local).AddTicks(309), "Yost - Hahn" },
                    { 93, 93, "Latin", "6340258017530", "Clothing", 3880, new DateTime(2023, 1, 21, 9, 43, 9, 417, DateTimeKind.Local).AddTicks(4182), "Langworth and Sons" },
                    { 94, 94, "Soul", "8902974037582", "array", 4729, new DateTime(2023, 1, 11, 4, 33, 51, 725, DateTimeKind.Local).AddTicks(6266), "Luettgen, Kovacek and Mante" },
                    { 95, 95, "Folk", "2400117416479", "Landing", 561, new DateTime(2022, 11, 28, 5, 58, 51, 576, DateTimeKind.Local).AddTicks(1287), "Cremin - Abbott" },
                    { 96, 96, "Blues", "9044968505871", "Persistent", 5190, new DateTime(2023, 1, 31, 11, 7, 52, 617, DateTimeKind.Local).AddTicks(2656), "Ward Group" },
                    { 97, 97, "Metal", "6098011764367", "Practical Granite Towels", 3372, new DateTime(2022, 7, 17, 13, 55, 13, 574, DateTimeKind.Local).AddTicks(8438), "Kerluke - Renner" },
                    { 98, 98, "World", "3358325895984", "Islands", 9145, new DateTime(2022, 12, 20, 22, 13, 27, 384, DateTimeKind.Local).AddTicks(830), "Kub Inc" },
                    { 99, 99, "Electronic", "2705952741701", "Outdoors & Automotive", 7785, new DateTime(2022, 12, 4, 3, 53, 21, 496, DateTimeKind.Local).AddTicks(475), "Purdy, Deckow and Yost" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookEntity_AuthorId",
                table: "BookEntity",
                column: "AuthorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookEntity");

            migrationBuilder.DropTable(
                name: "UserEntity");
        }
    }
}
