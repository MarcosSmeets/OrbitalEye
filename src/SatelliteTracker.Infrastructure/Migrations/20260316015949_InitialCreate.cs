using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SatelliteTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ground_stations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    altitude = table.Column<double>(type: "double precision", nullable: false),
                    country = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ground_stations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "satellites",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    norad_id = table.Column<int>(type: "integer", nullable: false),
                    international_designator = table.Column<string>(type: "text", nullable: true),
                    launch_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    @operator = table.Column<string>(name: "operator", type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_satellites", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orbits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    satellite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    inclination = table.Column<double>(type: "double precision", nullable: false),
                    eccentricity = table.Column<double>(type: "double precision", nullable: false),
                    raan = table.Column<double>(type: "double precision", nullable: false),
                    argument_of_perigee = table.Column<double>(type: "double precision", nullable: false),
                    mean_anomaly = table.Column<double>(type: "double precision", nullable: false),
                    mean_motion = table.Column<double>(type: "double precision", nullable: false),
                    epoch = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    tle_line1 = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    tle_line2 = table.Column<string>(type: "character varying(70)", maxLength: 70, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orbits", x => x.id);
                    table.ForeignKey(
                        name: "FK_orbits_satellites_satellite_id",
                        column: x => x.satellite_id,
                        principalTable: "satellites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "telemetry",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    satellite_id = table.Column<Guid>(type: "uuid", nullable: false),
                    timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    altitude = table.Column<double>(type: "double precision", nullable: false),
                    velocity = table.Column<double>(type: "double precision", nullable: false),
                    temperature = table.Column<double>(type: "double precision", nullable: true),
                    battery_level = table.Column<double>(type: "double precision", nullable: true),
                    signal_strength = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_telemetry", x => x.id);
                    table.ForeignKey(
                        name: "FK_telemetry_satellites_satellite_id",
                        column: x => x.satellite_id,
                        principalTable: "satellites",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orbits_satellite_id",
                table: "orbits",
                column: "satellite_id");

            migrationBuilder.CreateIndex(
                name: "IX_satellites_norad_id",
                table: "satellites",
                column: "norad_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_telemetry_satellite_id_timestamp",
                table: "telemetry",
                columns: new[] { "satellite_id", "timestamp" },
                descending: new[] { false, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ground_stations");

            migrationBuilder.DropTable(
                name: "orbits");

            migrationBuilder.DropTable(
                name: "telemetry");

            migrationBuilder.DropTable(
                name: "satellites");
        }
    }
}
