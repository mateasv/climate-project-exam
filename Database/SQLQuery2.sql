USE [TreeDB]
GO
/****** Object:  Table [dbo].[datalogger]    Script Date: 26-05-2022 16:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[datalogger](
	[datalogger_id] [int] IDENTITY(1,1) NOT NULL,
	[plant_id] [int] NULL,
	[min_air_humidity] [real] NULL,
	[max__air_humidity] [real] NULL,
	[min_air_temperature] [real] NULL,
	[max_air_temperature] [real] NULL,
 CONSTRAINT [PK_datalogger] PRIMARY KEY CLUSTERED 
(
	[datalogger_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[measurement]    Script Date: 26-05-2022 16:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[measurement](
	[measurement_id] [int] IDENTITY(1,1) NOT NULL,
	[datalogger_id] [int] NULL,
	[plant_id] [int] NULL,
	[soil_humidity] [real] NOT NULL,
	[air_humidity] [real] NOT NULL,
	[air_temerature] [real] NOT NULL,
	[soil_is_dry] [bit] NOT NULL,
 CONSTRAINT [PK_measurement] PRIMARY KEY CLUSTERED 
(
	[measurement_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[plant]    Script Date: 26-05-2022 16:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[plant](
	[plant_id] [int] IDENTITY(1,1) NOT NULL,
	[plant_type_id] [int] NULL,
	[warranty_start_date] [date] NULL,
	[price] [real] NULL,
 CONSTRAINT [PK_plant] PRIMARY KEY CLUSTERED 
(
	[plant_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[plant_type]    Script Date: 26-05-2022 16:46:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[plant_type](
	[plant_type_id] [int] IDENTITY(1,1) NOT NULL,
	[plant_type_name] [nchar](256) NULL,
 CONSTRAINT [PK_plant_type] PRIMARY KEY CLUSTERED 
(
	[plant_type_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[datalogger]  WITH CHECK ADD  CONSTRAINT [FK_datalogger_plant] FOREIGN KEY([plant_id])
REFERENCES [dbo].[plant] ([plant_id])
GO
ALTER TABLE [dbo].[datalogger] CHECK CONSTRAINT [FK_datalogger_plant]
GO
ALTER TABLE [dbo].[measurement]  WITH CHECK ADD  CONSTRAINT [FK_measurement_datalogger] FOREIGN KEY([datalogger_id])
REFERENCES [dbo].[datalogger] ([datalogger_id])
GO
ALTER TABLE [dbo].[measurement] CHECK CONSTRAINT [FK_measurement_datalogger]
GO
ALTER TABLE [dbo].[measurement]  WITH CHECK ADD  CONSTRAINT [FK_measurement_plant] FOREIGN KEY([plant_id])
REFERENCES [dbo].[plant] ([plant_id])
GO
ALTER TABLE [dbo].[measurement] CHECK CONSTRAINT [FK_measurement_plant]
GO
ALTER TABLE [dbo].[plant]  WITH CHECK ADD  CONSTRAINT [FK_plant_plant_type] FOREIGN KEY([plant_type_id])
REFERENCES [dbo].[plant_type] ([plant_type_id])
GO
ALTER TABLE [dbo].[plant] CHECK CONSTRAINT [FK_plant_plant_type]
GO
