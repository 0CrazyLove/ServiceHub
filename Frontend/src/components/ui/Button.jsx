import * as React from "react";
import { Slot } from "@radix-ui/react-slot";
import { cva } from "class-variance-authority";

import { cn } from "../../lib/utils";

const buttonVariants = cva(
    "inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-lg text-sm font-medium transition-all duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg]:size-4 [&_svg]:shrink-0",
    {
        variants: {
            variant: {
                default:
                    "bg-primary text-primary-foreground shadow-sm hover:bg-primary/90 active:scale-[0.98]",
                destructive:
                    "bg-destructive text-destructive-foreground shadow-sm hover:bg-destructive/90 active:scale-[0.98]",
                outline:
                    "border border-border bg-background hover:bg-muted hover:text-foreground active:scale-[0.98]",
                secondary:
                    "bg-secondary text-secondary-foreground shadow-sm hover:bg-secondary/80 active:scale-[0.98]",
                ghost:
                    "hover:bg-muted hover:text-foreground",
                link:
                    "text-primary underline-offset-4 hover:underline",
                hero:
                    "bg-primary text-primary-foreground shadow-lg hover:bg-primary/90 hover:shadow-xl active:scale-[0.98] font-semibold",
                subtle:
                    "bg-muted/50 text-foreground hover:bg-muted active:scale-[0.98]",
            },
            size: {
                default: "h-10 px-5 py-2",
                sm: "h-9 rounded-md px-4 text-xs",
                lg: "h-12 rounded-lg px-8 text-base",
                xl: "h-14 rounded-xl px-10 text-lg",
                icon: "h-10 w-10",
            },
        },
        defaultVariants: {
            variant: "default",
            size: "default",
        },
    },
);

const Button = React.forwardRef(
    ({ className, variant, size, asChild = false, ...props }, ref) => {
        const Comp = asChild ? Slot : "button";
        return (
            <Comp
                className={cn(buttonVariants({ variant, size, className }))}
                ref={ref}
                {...props}
            />
        );
    },
);
Button.displayName = "Button";

export { Button, buttonVariants };
