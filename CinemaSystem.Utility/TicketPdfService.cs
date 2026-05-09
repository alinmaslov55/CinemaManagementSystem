using CinemaSystem.Models.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Linq;
using System.Reflection.Metadata;

namespace CinemaSystem.Utility
{
    public class TicketPdfService : ITicketPdfService
    {
        public byte[] GenerateTicketPdfBytes(Booking booking)
        {
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily(Fonts.Arial));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("CINEMA SYSTEM").FontSize(24).SemiBold().FontColor(Colors.Blue.Darken2);
                        col.Item().Text($"Official Booking Receipt: {booking.ConfirmationCode}").FontSize(14).FontColor(Colors.Grey.Darken2);
                        col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Spacing(20);

                        col.Item().Text("Movie Tickets").FontSize(16).SemiBold().FontColor(Colors.Grey.Darken3);

                        foreach (var ticket in booking.Tickets)
                        {
                            col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten4).Padding(15).Row(row =>
                            {
                                row.RelativeItem().Column(ticketCol =>
                                {
                                    ticketCol.Item().Text(booking.Showtime.Movie.Title).FontSize(18).SemiBold();
                                    ticketCol.Item().Text($"Cinema: {booking.Showtime.CinemaHall.Cinema.Name} - Hall: {booking.Showtime.CinemaHall.Name}");
                                    ticketCol.Item().Text($"Date: {booking.Showtime.StartTime.ToString("dddd, MMM dd, yyyy - HH:mm")}");
                                });

                                row.ConstantItem(200).AlignRight().Column(ticketCol =>
                                {
                                    ticketCol.Item().Text($"SEAT {ticket.Seat.Row}{ticket.Seat.Column}").FontSize(20).Bold().FontColor(Colors.Red.Medium);
                                    ticketCol.Item().Text($"Type: {ticket.Seat.SeatType}");
                                    ticketCol.Item().Text($"Price: ${ticket.Price.ToString("F2")}");

                                    byte[] qrBytes = CinemaSystem.Utility.QRCodeHelper.GenerateQRCodeBytes(ticket.Barcode);

                                    ticketCol.Item().PaddingTop(10).Row(qrRow =>
                                    {
                                        qrRow.RelativeItem().AlignRight().PaddingRight(10).AlignMiddle().Text($"ID: {ticket.Barcode.Substring(0, 8).ToUpper()}").FontSize(10).FontColor(Colors.Grey.Medium);
                                        qrRow.ConstantItem(60).Height(60).Image(qrBytes);
                                    });
                                });
                            });
                        }

                        if (booking.BookingConcessions != null && booking.BookingConcessions.Any())
                        {
                            col.Item().PaddingTop(15).Text("Food & Beverage Vouchers").FontSize(16).SemiBold().FontColor(Colors.Orange.Darken2);

                            foreach (var concession in booking.BookingConcessions)
                            {
                                col.Item().Border(1).BorderColor(Colors.Orange.Lighten2).Background(Colors.White).Padding(15).Row(row =>
                                {
                                    row.ConstantItem(50).AlignMiddle().Text($"{concession.Quantity}x").FontSize(20).Bold().FontColor(Colors.Orange.Medium);

                                    row.RelativeItem().AlignMiddle().Column(cCol =>
                                    {
                                        cCol.Item().Text(concession.Concession.Name).FontSize(16).SemiBold();
                                        cCol.Item().Text("Present this voucher at the concession stand").FontSize(10).FontColor(Colors.Grey.Medium);
                                    });

                                    row.ConstantItem(100).AlignRight().AlignMiddle().Text($"${(concession.Quantity * concession.PriceAtPurchase).ToString("F2")}").FontSize(16).Bold().FontColor(Colors.Green.Darken2);
                                });
                            }
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated on ");
                        x.Span(DateTime.Now.ToString("g"));
                        x.Span($" | Total Amount Paid: ${booking.TotalAmount.ToString("F2")}").SemiBold();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}